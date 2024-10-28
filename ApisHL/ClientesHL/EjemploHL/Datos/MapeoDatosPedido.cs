using ClienteHL.Entidades.Consultas.Pedidos;
using EjemploHL.Utils;
using Microsoft.Data.SqlClient;
using static ClienteHL.Utils.Constantes;
using System.Data;
using System.Globalization;
using ClienteHL.Entidades.Operaciones;
using ClienteHL.Entidades.Consultas.Ordenes;
using System.Transactions;

namespace ClienteHL.Datos;

public interface IMapeoDatosPedido
{
	int ObtenerNumeroPedido(ConsultarPedido Valor);
	List<ListaPedidos> ListaDePedidos(ConsultarPedido Buscar);
	Operador ObtenerOperador(ConsultarPedido Valor);
	int Grabar(GrabarPedidosRequest Valor);
	int EliminarPedido(int IdPedido, int UsuarioEliminacion);
	DatosClienteCorreo datosClienteCorreo(int idOrden);
	DatosClienteCorreo datosClienteCorreoElim(int idPedido);
}

public class MapeoDatosPedido : IMapeoDatosPedido
{
	private readonly string connectionString;

	#region constructor
	public MapeoDatosPedido()
	{
		connectionString = Environment.GetEnvironmentVariable(StringHandler.DATABASELIQ)!;

		if (string.IsNullOrEmpty(connectionString))
		{
			throw new InvalidOperationException("La cadena de conexión no está configurada en las variables de entorno.");
		}

		// Configura el SqlConnectionStringBuilder con la cadena de conexión obtenida
		SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
		connectionString = builder.ConnectionString;
	}
	#endregion

	int IMapeoDatosPedido.ObtenerNumeroPedido(ConsultarPedido Valor)
	{
		return ObtenerNumeroPedido(Valor);
	}

	List<ListaPedidos> IMapeoDatosPedido.ListaDePedidos(ConsultarPedido Buscar)
	{
		return ObtenerListaPedidos(Buscar);
	}

	Operador IMapeoDatosPedido.ObtenerOperador(ConsultarPedido Valor)
	{
		return ObtenerOperador(Valor);
	}

	int IMapeoDatosPedido.Grabar(GrabarPedidosRequest Valor)
	{
		int result = 0;
		using (TransactionScope scope = new TransactionScope())
		{
			try
			{
				int estadoOrden = ObtenerIdDetalle(Estados.Orden, Estados.PorRecolectar);
				Valor.EstadoPedido = ObtenerIdDetalle(Estados.Pedido, Estados.PorRecolectar);
				int estadoPrueba = ObtenerIdDetalle(Estados.Prueba, Estados.PorRecolectar);
				int estadoMuestra = ObtenerIdDetalle(Estados.Muestra, Estados.PorRecolectar);
				int idPedido = GrabarPedido(Valor);
				int idObservacion = GrabarObservacion(Valor.UsuarioCreacion, Valor.Observacion, Valor.UsuarioCreacion, Valor.FechaCreacion);
				GrabarObservacionPedido(idPedido, idObservacion, Valor.UsuarioCreacion, Valor.FechaCreacion);

				foreach (var item in Valor.DatosOrden)
				{
					ActualizarOrden(item.IdOrden, estadoOrden, idPedido, Valor.UsuarioCreacion, Valor.FechaCreacion);
					List<Pruebas> pruebas = ListarPruebas(item.IdOrden);
					if (pruebas.Count > 0)
					{
						foreach (var item2 in pruebas)
						{
							ActualizarPrueba(item2.IdPrueba!.Value, estadoPrueba, Valor.UsuarioCreacion);
							List<PruebaMuestra> pruebaMuestra = ObtenerPruebaMuestra(item2.IdPrueba.Value);

							if (pruebaMuestra.Count > 0)
							{
								foreach (var item3 in pruebaMuestra)
								{
									ActualizarMuestra(item3.IdMuestraGalileo, estadoMuestra, Valor.UsuarioCreacion);
								}
							}
						}
					}
				}

				scope.Complete();
				result = Transaccion.Correcta;
			}
			catch (Exception ex)
			{
				scope.Dispose();
				result = Transaccion.Error;
			}
		};
		return result;
	}

	int IMapeoDatosPedido.EliminarPedido(int IdPedido, int UsuarioEliminacion)
	{
		using (TransactionScope scope = new TransactionScope())
		{
			try
			{
				int estadoPedido = ObtenerIdDetalle(Estados.Pedido, Estados.Anulado);
				int estadoOrden = ObtenerIdDetalle(Estados.Orden, Estados.Generada);
				int estadoPrueba = ObtenerIdDetalle(Estados.Prueba, Estados.Generada);

				BorrarPedido(IdPedido, estadoPedido, UsuarioEliminacion, DateTime.Now);

				List<ListarOrden> listaOrdenes = ListaDeOrdenes(IdPedido);
				if (listaOrdenes.Count > 0)
				{
					foreach (var item in listaOrdenes)
					{
						ActualizarOrden(item.NOrden, estadoOrden, UsuarioEliminacion, DateTime.Now);
						List<Pruebas> pruebas = ListarPruebas(item.NOrden);
						if (pruebas.Count > 0)
						{
							foreach (var item2 in pruebas)
							{
								ActualizarPrueba(item2.IdPrueba!.Value, estadoPrueba, UsuarioEliminacion);
							}
						}
					}
				}

				scope.Complete();
				return Transaccion.Correcta;
			}
			catch (Exception)
			{
				scope.Dispose();
				return Transaccion.Error;
			}
		}
	}

	DatosClienteCorreo IMapeoDatosPedido.datosClienteCorreo(int idOrden)
	{
		return ObtenerDatosCliente(idOrden);
	}

	DatosClienteCorreo IMapeoDatosPedido.datosClienteCorreoElim(int idPedido)
	{
		return ObtenerDatosElimCliente(idPedido);
	}

	private int ObtenerNumeroPedido(ConsultarPedido valor)
	{
		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(StringHandler.ProcedureNuevoPedido, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					//Envio de parametros
					command.Parameters.Add("@i_accion", SqlDbType.Char);
					command.Parameters["@i_accion"].Value = "C";

					command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
					command.Parameters["@usuarioCreacion"].Value = valor.IdUsuario;

					connection.Open();

					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataSet dataSet = new DataSet();
						adapter.Fill(dataSet);

						if (dataSet.Tables[0].Rows.Count > 0)
						{
							return _ = Convert.ToInt32(dataSet.Tables[0].Rows[0]["NumeroPedido"]);

						}
						else { return Transaccion.Default; }
					}
				}
			}
		}
		catch (Exception ex)
		{
			return Transaccion.Error;
		}
	}

	private List<ListaPedidos> ObtenerListaPedidos(ConsultarPedido Campo)
	{
		string FechaInicial = Campo.FechaDesde.Replace("d", "/");
		string FechaFinal = Campo.FechaHasta.Replace("d", "/");

		DateTime FechaI = DateTime.ParseExact(FechaInicial, "dd/MM/yyyy", CultureInfo.InvariantCulture);
		DateTime FechaF = DateTime.ParseExact(FechaFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture);

		string fechaFormateadaI = FechaI.ToString("yyyy-MM-dd");
		string fechaFormateadaF = FechaF.ToString("yyyy-MM-dd");

		List<ListaPedidos> lista = new List<ListaPedidos>();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureListarPedido, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@idUsuario", SqlDbType.Int);
				command.Parameters["@idUsuario"].Value = Campo.IdUsuario;

				command.Parameters.Add("@opcionBusqueda", SqlDbType.Int);
				command.Parameters["@opcionBusqueda"].Value = Campo.OpcionBusqueda;

				command.Parameters.Add("@datoBusqueda", SqlDbType.VarChar);
				command.Parameters["@datoBusqueda"].Value = Campo.DatoBusqueda;

				command.Parameters.Add("@numRemision", SqlDbType.VarChar);
				if (Campo.numRemision == null || Campo.numRemision == "")
				{
					Campo.numRemision = "";					
					command.Parameters["@numRemision"].Value = Campo.numRemision.ToString();					
				}
				else
				{					
					command.Parameters["@numRemision"].Value = Campo.numRemision;					
				}

				command.Parameters.Add("@fechaInicio", SqlDbType.Date);
				command.Parameters["@fechaInicio"].Value = fechaFormateadaI.ToString();

				command.Parameters.Add("@fechaFin", SqlDbType.Date);
				command.Parameters["@fechaFin"].Value = fechaFormateadaF.ToString();

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count > 0)
					{
						lista = dataSet.Tables[0].AsEnumerable().Select(dataRow => new ListaPedidos
						{
							IdPedido = dataRow.Field<int>("IdPedido"),
							NumeroRemision = dataRow.Field<string>("NumeroRemision")!,
							FechaCreacion = dataRow.Field<DateTime>("FechaCreacion"),
							TotalOrdenes = dataRow.Field<int>("TotalOrdenes"),
							TotalMuestras = dataRow.Field<int>("TotalMuestras"),
							TotalRetiradas = dataRow.Field<int>("TotalRetiradas"),
							FechaRetiro = dataRow.Field<DateTime>("FechaRetiro"),
							EstadoPedido = dataRow.Field<string>("EstadoPedido")!
						}).ToList();

						return lista;
					}
					else { return lista; }
				}
			}
		}							
	}

	private Operador ObtenerOperador(ConsultarPedido Valor)
	{
		Operador operador = new Operador();

		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(StringHandler.ProcedureOperador, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					//Envio de parametros
					command.Parameters.Add("@i_accion", SqlDbType.Char);
					command.Parameters["@i_accion"].Value = "C";

					command.Parameters.Add("@idUsuario", SqlDbType.Int);
					command.Parameters["@idUsuario"].Value = Valor.IdUsuario;

					connection.Open();

					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataSet dataSet = new DataSet();
						adapter.Fill(dataSet);

						if (dataSet.Tables[0].Rows.Count > 0)
						{
							operador.IdOperador = Convert.ToInt32(dataSet.Tables[0].Rows[0]["IdOperador"]);
							operador.Nombre = dataSet.Tables[0].Rows[0]["Nombre"].ToString()!;

							return operador;
						}
						else { return operador; }
					}
				}
			}
		}
		catch (Exception ex)
		{
			return operador;
		}
	}

	private int ObtenerIdDetalle(string Estado, string Valor)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureIdDetalle, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@estado", SqlDbType.VarChar);
				command.Parameters["@estado"].Value = Estado;

				command.Parameters.Add("@valor", SqlDbType.VarChar);
				command.Parameters["@valor"].Value = Valor;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);
					
					if (dataSet.Tables[0].Rows.Count > 0)
					{
						return _ = Convert.ToInt32(dataSet.Tables[0].Rows[0]["IdDetalle"]);
					}
					else { return 0; }
				}
			}
		}
	}

	private int GrabarPedido(GrabarPedidosRequest valor)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureNuevoPedido, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "I";

				command.Parameters.Add("@idPedido", SqlDbType.Int);
				command.Parameters["@idPedido"].Value = valor.IdPedido;

				command.Parameters.Add("@idOperador", SqlDbType.Int);
				command.Parameters["@idOperador"].Value = valor.IdOperador;

				command.Parameters.Add("@usuarioOperador", SqlDbType.VarChar);
				command.Parameters["@usuarioOperador"].Value = valor.UsuarioOperador;

				command.Parameters.Add("@numeroRemision", SqlDbType.VarChar);
				command.Parameters["@numeroRemision"].Value = valor.NumeroRemision;

				command.Parameters.Add("@estadoPedido", SqlDbType.Int);
				command.Parameters["@estadoPedido"].Value = valor.EstadoPedido;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = valor.UsuarioCreacion;

				command.Parameters.Add("@fechaCreacion", SqlDbType.Date);
				command.Parameters["@fechaCreacion"].Value = valor.FechaCreacion;

				command.Parameters.Add("@idCliente", SqlDbType.Int);
				command.Parameters["@idCliente"].Value = valor.IdClientePedido;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet idPedido = new DataSet();
					adapter.Fill(idPedido);

					if (idPedido.Tables[0].Rows.Count > 0)
					{
						return _ = Convert.ToInt32(idPedido.Tables[0].Rows[0]["idPedido"]);
					}
					else { return Transaccion.Default; }
				}
			}
		}							
	}

	private int GrabarObservacion(int IdUsuario, string Descripcion, int UsuarioCreacion, DateTime FechaCreacion)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureObservacion, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "I";

				command.Parameters.Add("@descripcion", SqlDbType.VarChar);
				command.Parameters["@descripcion"].Value = Descripcion;
				//command.Parameters["@descripcion"].Value = string.IsNullOrWhiteSpace(Descripcion) ? "Sin observación" : Descripcion;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = UsuarioCreacion;

				command.Parameters.Add("@fechaCreacion", SqlDbType.Date);
				command.Parameters["@fechaCreacion"].Value = FechaCreacion;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet idObservacion = new DataSet();
					adapter.Fill(idObservacion);

					if (idObservacion.Tables[0].Rows.Count > 0)
					{
						return _ = Convert.ToInt32(idObservacion.Tables[0].Rows[0]["idObservacion"]);

					}
					else { return Transaccion.Default; }
				}
			}
		}
	}

	//Verificar despues
	private void GrabarObservacionPedido(int IdPedido, int IdObservacion, int UsuarioCreacion, DateTime FechaCreacion)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureObservacionPedido, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "I";

				command.Parameters.Add("@idPedido", SqlDbType.Int);
				command.Parameters["@idPedido"].Value = IdPedido;

				command.Parameters.Add("@idObservacion", SqlDbType.Int);
				command.Parameters["@idObservacion"].Value = IdObservacion;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = UsuarioCreacion;

				command.Parameters.Add("@fechaCreacion", SqlDbType.Date);
				command.Parameters["@fechaCreacion"].Value = FechaCreacion;

				connection.Open();
				command.ExecuteNonQuery();
			}
		}		
	}

	//verificar despues
	private void ActualizarOrden(int IdOrden, int Estado, int IdPedido, int UsuarioCreacion, DateTime FechaCreacion)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureActualizarOrden, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "M";

				command.Parameters.Add("@idOrden", SqlDbType.Int);
				command.Parameters["@idOrden"].Value = IdOrden;

				command.Parameters.Add("@idPedido", SqlDbType.Int);
				command.Parameters["@idPedido"].Value = IdPedido;

				command.Parameters.Add("@estadoOrden", SqlDbType.Int);
				command.Parameters["@estadoOrden"].Value = Estado;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = UsuarioCreacion;

				command.Parameters.Add("@fechaCreacion", SqlDbType.Date);
				command.Parameters["@fechaCreacion"].Value = FechaCreacion;

				connection.Open();
				command.ExecuteNonQuery();
			}
		}		
	}


	//verificar despues
	private void ActualizarOrden(int IdOrden, int Estado, int UsuarioCreacion, DateTime FechaCreacion)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureActualizarOrden, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "M";

				command.Parameters.Add("@idOrden", SqlDbType.Int);
				command.Parameters["@idOrden"].Value = IdOrden;

				command.Parameters.Add("@estadoOrden", SqlDbType.Int);
				command.Parameters["@estadoOrden"].Value = Estado;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = UsuarioCreacion;

				command.Parameters.Add("@fechaCreacion", SqlDbType.Date);
				command.Parameters["@fechaCreacion"].Value = FechaCreacion;

				connection.Open();
				command.ExecuteNonQuery();
			}
		}							
	}

	private List<Pruebas> ListarPruebas(int IdOrden)
	{
		List<Pruebas> lista = new List<Pruebas>();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedurePrueba, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C1";

				command.Parameters.Add("@idOrden2", SqlDbType.Int);
				command.Parameters["@idOrden2"].Value = IdOrden;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count > 0)
					{
						lista = dataSet.Tables[0].AsEnumerable().Select(dataRow => new Pruebas
						{
							IdOrden = dataRow.Field<int>("IdOrden"),
							IdPrueba = dataRow.Field<int>("IdPrueba"),
							IdPruebaGalileo = dataRow.Field<int>("IdPruebaGalileo")

						}).ToList();

						return lista;
					}
					else { return lista; }
				}
			}
		}				
	}

	//verificar despues
	private void ActualizarPrueba(int idPrueba, int estadoPrueba, int usuarioCreacion)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedurePrueba, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "M";

				command.Parameters.Add("@idPrueba", SqlDbType.Int);
				command.Parameters["@idPrueba"].Value = idPrueba;

				command.Parameters.Add("@estado", SqlDbType.Int);
				command.Parameters["@estado"].Value = estadoPrueba;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = usuarioCreacion;

				command.Parameters.Add("@fechaCreacion", SqlDbType.DateTime);
				command.Parameters["@fechaCreacion"].Value = DateTime.Now;

				connection.Open();
				command.ExecuteNonQuery();
			}
		}					
	}

	private List<PruebaMuestra> ObtenerPruebaMuestra(int IdPrueba)
	{
		List<PruebaMuestra> lista = new List<PruebaMuestra>();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedurePruebaMuestra, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C1";

				command.Parameters.Add("@idPrueba", SqlDbType.Int);
				command.Parameters["@idPrueba"].Value = IdPrueba;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count > 0)
					{
						lista = dataSet.Tables[0].AsEnumerable().Select(dataRow => new PruebaMuestra
						{
							IdPruebaGalileo = dataRow.Field<int>("IdPrueba"),
							IdMuestraGalileo = dataRow.Field<int>("IdMuestra")

						}).ToList();

						return lista;
					}
					else { return lista; }
				}
			}
		}			
	}

	//verificar despues
	private void ActualizarMuestra(int idMuestra, int estadoMuestra, int usuarioCreacion)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureMuestra, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "M";

				command.Parameters.Add("@idMuestra", SqlDbType.Int);
				command.Parameters["@idMuestra"].Value = idMuestra;

				command.Parameters.Add("@estadoMuestra", SqlDbType.Int);
				command.Parameters["@estadoMuestra"].Value = estadoMuestra;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = usuarioCreacion;

				connection.Open();
				command.ExecuteNonQuery();
			}
		}		
	}

	//verificar despues
	private void BorrarPedido(int IdPedido, int EstadoPedido, int UsuarioEliminacion, DateTime FechaEliminacion)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureNuevoPedido, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "E1";

				command.Parameters.Add("@idPedido", SqlDbType.Int);
				command.Parameters["@idPedido"].Value = IdPedido;

				command.Parameters.Add("@estadoPedido", SqlDbType.Int);
				command.Parameters["@estadoPedido"].Value = EstadoPedido;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = UsuarioEliminacion;

				command.Parameters.Add("@fechaCreacion", SqlDbType.Date);
				command.Parameters["@fechaCreacion"].Value = FechaEliminacion;

				connection.Open();
				command.ExecuteNonQuery();
			}
		}					
	}

	private List<ListarOrden> ListaDeOrdenes(int idPedido)
	{
		List<ListarOrden> lista = new List<ListarOrden>();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureConsultarOrden, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C5";

				command.Parameters.Add("@idPedido", SqlDbType.Int);
				command.Parameters["@idPedido"].Value = idPedido;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count > 0)
					{
						lista = dataSet.Tables[0].AsEnumerable().Select(dataRow => new ListarOrden
						{
							NOrden = dataRow.Field<int>("IdOrden"),
							CodigoBarra = dataRow.Field<string>("CodigoBarra")!,

						}).ToList();

						return lista;
					}
					else { return lista; }
				}
			}
		}					
	}

	public DatosClienteCorreo ObtenerDatosCliente(int idOrden)
	{
		DatosClienteCorreo datosCliente = new DatosClienteCorreo();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureDatosCliente, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@idOrden", SqlDbType.Int);
				command.Parameters["@idOrden"].Value = idOrden;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						datosCliente.Cliente = dataSet.Tables[0].Rows[0]["Cliente"].ToString()!;
						datosCliente.Telefono = dataSet.Tables[0].Rows[0]["Telefono"].ToString()!;
						datosCliente.DireccionCliente = dataSet.Tables[0].Rows[0]["DireccionCliente"].ToString()!;
						datosCliente.Correo = dataSet.Tables[0].Rows[0]["Correo"].ToString()!;
						datosCliente.TotalMuestras = dataSet.Tables[1].Rows[0]["totalMuestras"].ToString()!;

						return datosCliente;
					}
					else
					{
						return datosCliente;
					}
				}
			}
		}		
	}

	public DatosClienteCorreo ObtenerDatosElimCliente(int idPedido)
	{
		DatosClienteCorreo datosCliente = new DatosClienteCorreo();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureDatosClienteElim, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@i_idPedido", SqlDbType.Int);
				command.Parameters["@i_idPedido"].Value = idPedido;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						datosCliente.Cliente = dataSet.Tables[0].Rows[0]["Cliente"].ToString()!;
						datosCliente.Telefono = dataSet.Tables[0].Rows[0]["Telefono"].ToString()!;
						datosCliente.DireccionCliente = dataSet.Tables[0].Rows[0]["DireccionCliente"].ToString()!;
						datosCliente.Correo = dataSet.Tables[0].Rows[0]["Correo"].ToString()!;
						datosCliente.numRemision = dataSet.Tables[1].Rows[0]["NumeroRemision"].ToString()!;

						return datosCliente;
					}
					else
					{
						return datosCliente;
					}
				}
			}
		}
	}
}
