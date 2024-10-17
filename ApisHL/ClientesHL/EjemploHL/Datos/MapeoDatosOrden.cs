using ClienteHL.Entidades.Consultas.CatalogoMaestro;
using ClienteHL.Entidades.Consultas.Medico;
using ClienteHL.Entidades.Consultas.Ordenes;
using ClienteHL.Entidades.Consultas.Resultados;
using ClienteHL.Entidades.Operaciones;
using EjemploHL.Entidades.Consultas.CatalogoMaestro;
using EjemploHL.Utils;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Transactions;
using System.Windows.Input;
using static ClienteHL.Utils.Constantes;

namespace ClienteHL.Datos;

public interface IMapeoDatosOrden
{
	int ObtenerNumeroOrden(ConsultarOrden V);
	GrabarOrdenRequest ObtenerOrden(int IdOrden);
	int GetIdOrden(string Valor);
	List<Pruebas> ListarPruebas(int IdOrden);
	List<LogObservaciones> Observaciones(string CodigoBarra);
	List<ListarOrden> ListarOrdenes(ConsultarOrden query);
	List<Muestras> ListaMuestra(int IdOrden);
	int Grabar(GrabarOrdenRequest request);
	int Actualizar(GrabarOrdenRequest request);
	int Eliminar(GrabarOrdenRequest request);
	int EliminarPruebas(Pruebas request);
	int GrabarResultados(string CodigoBarra, string Resultados, int UsuarioCreacion);
	string ConsultarResultados(string CodigoBarra);
	string nombreEstadoOrden(string idOrdenEstado);
	string ActuOrdenPdfResult(ActualizaPdfOrden actualizaPdfOrden);
	int CLienteIdOrden(int idGalileo, string accion);
	int CLienteIdPedido(int id);
	string ActualizaResultadoFinal(string codOrdenHuma, string codOrdenLis,
						int idLab, string pdfBase64, int idOrden);
	string ConsultarPdfFinal(string codBarra);
	string InsertarResultadosLab(ResultadosRequest resultadosRequest, string Informe);
	List<ResultadosMedico> ResultadosLabMedico(ConsultarResultados consultar);
	string ConsultarPdfFinalResult(string codBarra, int idResult);
	string ConsultResultPdfNew(int idLab, string codOrden);
	Task<string> ConsultarResultadosPaciente(string Identificacion, int idLab);

}


public class MapeoDatosOrden: IMapeoDatosOrden
{
	private readonly string connectionString;

	int IMapeoDatosOrden.ObtenerNumeroOrden(ConsultarOrden Valor)
	{
		return ObtenerNumerOrden(Valor);
	}

	GrabarOrdenRequest IMapeoDatosOrden.ObtenerOrden(int IdOrden)
	{
		GrabarOrdenRequest grabarOrdenRequest = new GrabarOrdenRequest();

		grabarOrdenRequest = ConsultarOrden(IdOrden);
		grabarOrdenRequest.Paciente = ObtenerPaciente(grabarOrdenRequest.Identificacion!);
		return grabarOrdenRequest;
	}

	int IMapeoDatosOrden.GetIdOrden(string Valor)
	{
		return NumeroOrden(Valor);
	}

	List<Pruebas> IMapeoDatosOrden.ListarPruebas(int IdOrden)
	{
		List<Pruebas> prueba = new List<Pruebas>();
		prueba = ConsultarPruebas(IdOrden);

		return prueba;
	}

	List<LogObservaciones> IMapeoDatosOrden.Observaciones(string CodigoBarra)
	{
		List<LogObservaciones> obs = new List<LogObservaciones>();
		obs = ListaObservaciones(CodigoBarra);

		return obs;
	}

	List<ListarOrden> IMapeoDatosOrden.ListarOrdenes(ConsultarOrden Valor)
	{
		return ListarOrdenes(Valor);
	}

	List<Muestras> IMapeoDatosOrden.ListaMuestra(int IdOrden)
	{
		return ListarMuestras(IdOrden);
	}

	int IMapeoDatosOrden.Grabar(GrabarOrdenRequest request)
	{
		int result = 0;
		using (TransactionScope scope = new TransactionScope())
		{
			try
			{
				request.Estado = ObtenerIdDetalle(Estados.Orden, Estados.Generada);
				GrabarPaciente(request.Paciente);
				request.IdOrden = GrabarOrden(request);
				int incremento = 1;
				foreach (var prueba in request.Pruebas)
				{
					prueba.IdOrden = request.IdOrden;
					prueba.EstadoPrueba = ObtenerIdDetalle(Estados.Prueba, Estados.Generada);
					int CodigoPrueba = GrabarPrueba(prueba);
					int CodigoMuestra = ExisteMuestra(prueba.IdOrden.Value, prueba.IdMuestraGalileo!.Value);

					if (CodigoMuestra == 0)
					{
						//CodigoMuestra = GrabarMuestra(prueba.IdMuestraGalileo, prueba.IdOrden, prueba.NombreMuestra,prueba.MuestraAlterna, prueba.Recipiente, prueba.CodigoBarra + Caracteres.Guion + incremento, ObtenerIdDetalle(Estados.Muestra, Estados.PorRecolectar), prueba.UsuarioCreacion, prueba.FechaCreacion);
						CodigoMuestra = GrabarMuestra(prueba.IdMuestraGalileo.Value, prueba.IdOrden.Value, prueba.NombreMuestra!, prueba.MuestraAlterna!, prueba.Recipiente!, prueba.CodigoBarra + Caracteres.Guion + prueba.IdMuestraGalileo, ObtenerIdDetalle(Estados.Muestra, Estados.PorRecolectar), prueba.UsuarioCreacion!.Value, prueba.FechaCreacion);
						incremento++;
					}
					GrabarPruebaMuestra(CodigoPrueba, CodigoMuestra, prueba.UsuarioCreacion!.Value, prueba.FechaCreacion);
				}

				scope.Complete();
				scope.Dispose();
				result = Transaccion.Correcta;
			}
			catch (Exception)
			{
				scope.Dispose();
				result = Transaccion.Error;
			}

		}
		return result;
	}

	int IMapeoDatosOrden.Actualizar(GrabarOrdenRequest request)
	{
		int result = 0;
		using (TransactionScope scope = new TransactionScope())
		{
			try
			{
				//verifica el valor del estado orden y prueba para cambiar
				//dependiendo del estado que este la orden
				string valorEstado = ObtenerValEstaOrdenPrueba(request.Estado!.Value, Convert.ToChar("C"));

				//actualiza a generada
				if (valorEstado == "GENE")
				{
					request.Estado = ObtenerIdDetalle(Estados.Orden, Estados.Generada);
				}
				//actualiza a por recolectar
				if (valorEstado == "PREC")
				{
					request.Estado = ObtenerIdDetalle(Estados.Orden, Estados.PorRecolectar);
				}

				ActualizarPaciente(request.Paciente);
				request.IdOrden = ObtenerIdOrden(request.CodigoBarra!);

				//Proceso Actualizar Orden
				ActualizarOrden(request);

				int incremento = CantidadMuestra(request.IdOrden.Value);

				foreach (var prueba in request.Pruebas)
				{
					prueba.IdOrden = request.IdOrden;
					prueba.EstadoPrueba = ObtenerIdDetalle(Estados.Prueba, Estados.Generada);
					//prueba.EstadoPrueba = ObtenerIdDetalle(Estados.Prueba, Estados.PorRecolectar);

					//actualiza a generada
					//if (valorEstado == "GENE")
					//{
					//    prueba.EstadoPrueba = ObtenerIdDetalle(Estados.Prueba, Estados.Generada);
					//}
					//actualiza a por recolectar
					//if (valorEstado == "PREC")
					//{
					//    prueba.EstadoPrueba = ObtenerIdDetalle(Estados.Prueba, Estados.PorRecolectar);
					//}

					int existe = ExistePruebas(prueba.IdOrden.Value, prueba.IdPruebaGalileo!.Value);

					if (existe == 0)
					{
						int CodigoPrueba = GrabarPrueba(prueba);
						int CodigoMuestra = ExisteMuestra(prueba.IdOrden.Value, prueba.IdMuestraGalileo!.Value);
						if (CodigoMuestra == 0)
						{
							incremento++;
							CodigoMuestra = GrabarMuestra(prueba.IdMuestraGalileo.Value, prueba.IdOrden.Value, prueba.NombreMuestra!, prueba.MuestraAlterna!, prueba.Recipiente!, prueba.CodigoBarra + Caracteres.Guion + incremento, ObtenerIdDetalle(Estados.Muestra, Estados.PorRecolectar), prueba.UsuarioCreacion!.Value, prueba.FechaCreacion);
						}

						GrabarPruebaMuestra(CodigoPrueba, CodigoMuestra, prueba.UsuarioCreacion!.Value, prueba.FechaCreacion);
					}
				}

				scope.Complete();
				scope.Dispose();
				result = Transaccion.Correcta;
			}
			catch (Exception)
			{
				scope.Dispose();
				result = Transaccion.Error;
			}
		}
		return result;
	}

	int IMapeoDatosOrden.Eliminar(GrabarOrdenRequest request)
	{
		int result = 0;
		using (TransactionScope scope = new TransactionScope())
		{
			try
			{
				request.Estado = ObtenerIdDetalle(Estados.Orden, Estados.Cancelada);
				int estadoPrueba = ObtenerIdDetalle(Estados.Prueba, Estados.Cancelada);
				int estadoMuestra = ObtenerIdDetalle(Estados.Muestra, Estados.Cancelada);
				EliminarOrden(request);
				List<Pruebas> pruebas = ListarPruebas(request.IdOrden!.Value);

				if (pruebas.Count > 0)
				{
					foreach (var item in pruebas)
					{
						EliminarPrueba(item.IdPrueba!.Value, estadoPrueba, request.UsuarioCreacion!.Value);
						List<PruebaMuestra> pruebaMuestra = ObtenerPruebaMuestra(item.IdPrueba.Value);

						if (pruebaMuestra.Count > 0)
						{
							foreach (var item2 in pruebaMuestra)
							{

								EliminarPruebaMuestra(item2.IdMuestraGalileo, item2.IdPruebaGalileo, request.UsuarioCreacion.Value);
								EliminarMuestra(item2.IdMuestraGalileo, request.IdOrden.Value, estadoMuestra, request.UsuarioCreacion.Value);
							}
						}
					}
				}

				scope.Complete();
				scope.Dispose();
				result = Transaccion.Correcta;
			}
			catch (Exception)
			{
				scope.Dispose();
				result = Transaccion.Error;
			}
		}
		return result;
	}

	int IMapeoDatosOrden.EliminarPruebas(Pruebas request)
	{
		int result = Transaccion.Default;
		using (TransactionScope scope = new TransactionScope())
		{
			try
			{
				int existe = ExistePruebas(request.IdOrden!.Value, request.IdPruebaGalileo!.Value);
				int estadoOrden = ObtenerIdDetalle(Estados.Orden, Estados.Cancelada);
				int estadoPrueba = ObtenerIdDetalle(Estados.Prueba, Estados.Cancelada);
				int estadoMuestra = ObtenerIdDetalle(Estados.Muestra, Estados.Cancelada);
				if (existe > Numeros.Cero)
				{

					int CodigoPrueba = GetIdPrueba(request.IdOrden.Value, request.IdPruebaGalileo.Value);
					int CodigoPruebaMuestra = GetIdPruebaMuestra(CodigoPrueba);
					int CodigoMuestra = GetIdMuestra(CodigoPruebaMuestra);

					EliminarPrueba(CodigoPrueba, estadoPrueba, request.UsuarioCreacion!.Value);
					EliminarPruebaMuestra(CodigoMuestra, CodigoPrueba, request.UsuarioCreacion.Value);
					if (ContarPruebaMuestra(CodigoMuestra) < Numeros.Dos)
					{
						EliminarMuestra(CodigoMuestra, request.IdOrden.Value, estadoMuestra, request.UsuarioCreacion.Value);
					}
				}

				scope.Complete();
				scope.Dispose();
				result = Transaccion.Correcta;
			}
			catch (Exception)
			{
				scope.Dispose();
				result = Transaccion.Error;
			}
		}

		return result;
	}

	int IMapeoDatosOrden.GrabarResultados(string CodigoBarra, string Resultados, int UsuarioCreacion)
	{
		int result = Transaccion.Default;
		using (TransactionScope scope = new TransactionScope())
		{
			try
			{
				List<Pruebas> lista = new List<Pruebas>();
				int estado = ObtenerIdDetalle(Estados.Orden, Estados.Validado);
				int estadoPrueba = ObtenerIdDetalle(Estados.Prueba, Estados.Validado);
				int idOrden = GuardarResultados(CodigoBarra, Resultados, estado, UsuarioCreacion);

				int EstadoRechazada = ObtenerIdDetalle(Estados.Prueba, Estados.Rechazada);
				int EstadoCancelada = ObtenerIdDetalle(Estados.Prueba, Estados.Cancelada);
				int estadoNoProcesada = ObtenerIdDetalle(Estados.Prueba, Estados.NoProcesada);

				lista = ListPruebas(idOrden);

				foreach (var item in lista)
				{
					if (item.EstadoPrueba != EstadoRechazada || item.EstadoPrueba != estadoNoProcesada || item.EstadoPrueba != EstadoCancelada)
					{
						ActualizarPruebasOrden(item.IdPrueba!.Value, estadoPrueba, UsuarioCreacion);
					}
				}

				scope.Complete();
				scope.Dispose();
				result = Transaccion.Correcta;
			}
			catch (Exception)
			{
				scope.Dispose();
				result = Transaccion.Error;
			}
		}
		return result;
	}

	string IMapeoDatosOrden.ConsultarResultados(string CodigoBarra)
	{
		return Base64Resultados(CodigoBarra);
	}

	string IMapeoDatosOrden.nombreEstadoOrden(string idOrdenEstado)
	{
		return NombreEstadoOrden(idOrdenEstado);
	}

	string IMapeoDatosOrden.ActuOrdenPdfResult(ActualizaPdfOrden actualizaPdfOrden)
	{
		return OrdenActuaPdfResult(actualizaPdfOrden);
	}

	int IMapeoDatosOrden.CLienteIdOrden(int idGalielo, string accion)
	{
		return ObtenerIdClienteOrden(idGalielo, accion);
	}

	int IMapeoDatosOrden.CLienteIdPedido(int id)
	{
		return ObtenerIdClientePedido(id);
	}

	string IMapeoDatosOrden.ActualizaResultadoFinal(string codOrdenHuma, string codOrdenLis,
				int idLab, string pdfBase64, int idOrden)
	{
		return OrdenActuaPdfResultFinal(codOrdenHuma, codOrdenLis,
						idLab, pdfBase64, idOrden);
	}

	string IMapeoDatosOrden.ConsultarPdfFinal(string codBarra)
	{
		return ConsultarBaseResultados(codBarra);
	}

	string IMapeoDatosOrden.InsertarResultadosLab(ResultadosRequest resultadosRequest, string Informe)
	{
		return ResultadosInsertar(resultadosRequest, Informe);
	}

	List<ResultadosMedico> IMapeoDatosOrden.ResultadosLabMedico(ConsultarResultados consultar)
	{
		return ListaResultadosMedicoLab(consultar);
	}

	string IMapeoDatosOrden.ConsultarPdfFinalResult(string codBarra, int idResult)
	{
		return PdfFinalResultMedico(codBarra, idResult);
	}

	string IMapeoDatosOrden.ConsultResultPdfNew(int idLab, string codOrden)
	{
		return ResultPdfNew(idLab, codOrden);
	}

	public async Task<string> ConsultarResultadosPaciente(string Identificacion, int idLab)
	{
		return await ConsultPacienteResult(Identificacion, idLab);
	}

	#region Constructor
	public MapeoDatosOrden()
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

	private int ObtenerNumerOrden(ConsultarOrden valor)
	{
		int NumeroOrden = 0;
		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(StringHandler.ProcedureNuevaOrden, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					//Envio de parametros
					command.Parameters.Add("@i_accion", SqlDbType.Char);
					command.Parameters["@i_accion"].Value = "C";

					command.Parameters.Add("@idUsuarioGalileo", SqlDbType.Int);
					command.Parameters["@idUsuarioGalileo"].Value = valor.IdUsuarioGalileo;

					connection.Open();

					//lectura de la data
					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataSet dataSet = new DataSet();
						adapter.Fill(dataSet);

						if (dataSet.Tables.Count == 0) return 0;
						if (dataSet.Tables[0].Rows.Count == 0) return 0;

						NumeroOrden = Convert.ToInt32(dataSet.Tables[0].Rows[0]["NumeroOrden"]);

						return NumeroOrden;
					}
				}
			}
		}
		catch(Exception ex)
		{
			return 400;
		}
	}

	private GrabarOrdenRequest ConsultarOrden(int IdOrden)
	{
		GrabarOrdenRequest orden = new GrabarOrdenRequest();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureConsultarOrden, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@idOrden", SqlDbType.Int);
				command.Parameters["@idOrden"].Value = IdOrden;

				connection.Open();

				//lectura de la data
				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					orden = ConvertTo<GrabarOrdenRequest>(dataSet.Tables[0]);

					return orden;
				}
			}
		}
	}

	private Pacientes ObtenerPaciente(string Identificacion)
	{
		Pacientes Paciente = new Pacientes();

		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(StringHandler.ProcedurePaciente, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					//Envio de parametros
					command.Parameters.Add("@i_accion", SqlDbType.Char);
					command.Parameters["@i_accion"].Value = "C";

					command.Parameters.Add("@identificacion", SqlDbType.VarChar);
					command.Parameters["@identificacion"].Value = Identificacion;

					connection.Open();

					//lectura de data
					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataSet dataSet = new DataSet();
						adapter.Fill(dataSet);

						if (dataSet.Tables.Count == 0) return null!;
						if (dataSet.Tables[0].Rows.Count == 0)
						{
							var Paciente1 = new Pacientes
							{
								Identificacion = null!,
								Nombres = null!,
								Apellidos = null!,
								Genero = false,
								FechaNacimiento = DateTime.Now,
								Edad = null!,
								Telefono = null!,
								Email = null!,
								UsuarioCreacion = 0,
								FechaCreacion = DateTime.Now,
								TipoPaciente = 0,
								CodLaboratorio = null!
							};

							return Paciente1;
						}
						else
						{
							Paciente = ConvertTo<Pacientes>(dataSet.Tables[0]);
							return Paciente;
						}
					}
				}
			}
		}
		catch(Exception ex)
		{
			return null!;
		}
	}

	private int NumeroOrden(string Valor)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using(SqlCommand command = new SqlCommand(StringHandler.ProcedureConsultarOrden, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C4";

				command.Parameters.Add("@codigoBarra", SqlDbType.VarChar);
				command.Parameters["@codigoBarra"].Value = Valor;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						return _ = Convert.ToInt32(dataSet.Tables[0].Rows[0]["IdOrden"]);
					}
					else
					{
						return Transaccion.Default;
					}
				}
			}
		}
	}

	private List<Pruebas> ConsultarPruebas(int IdOrden)
	{
		List<Pruebas> lista = new List<Pruebas>();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureConsultarOrden, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C1";

				command.Parameters.Add("@idOrden", SqlDbType.Int);
				command.Parameters["@idOrden"].Value = IdOrden;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if(dataSet.Tables.Count > 0)
					{
						lista = dataSet.Tables[0].AsEnumerable().Select(dataRow => new Pruebas
						{
							IdOrden = dataRow.Field<int?>("IdOrden"),
							IdPrueba = dataRow.Field<int?>("IdPrueba"),
							IdPruebaGalileo = dataRow.Field<int?>("IdPruebaGalileo"),
							IdMuestraGalileo = dataRow.Field<int?>("IdMuestraGalileo"),
							CodigoExamen = dataRow.Field<string>("CodigoExamen") ?? string.Empty,
							EsPerfil = dataRow.Field<bool>("EsPerfil"),
							Nombre = dataRow.Field<string>("Nombre") ?? string.Empty,
							Abreviatura = dataRow.Field<string>("Abreviatura") ?? string.Empty,
							Metodologia = dataRow.Field<string>("Metodologia") ?? string.Empty,
							NombreMuestra = dataRow.Field<string>("NombreMuestra") ?? string.Empty,
							MuestraAlterna = dataRow.Field<string>("MuestraAlterna") ?? string.Empty,
							Recipiente = dataRow.Field<string>("Recipiente") ?? string.Empty,
                            //Precio = float.Parse(dataRow.Field<decimal>("Precio").ToString())                            
                            Precio = ConvertDecimalToFloat(dataRow.Field<decimal?>("Precio"))

                        }).ToList();

						return lista;
					}
					else
					{
						return lista;
					}
				}
			}
		}
	}

	private List<LogObservaciones> ListaObservaciones(string CodigoBarra)
	{
		List<LogObservaciones> lista = new List<LogObservaciones>();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureConsultarOrden, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@codigoBarra", SqlDbType.VarChar);
				command.Parameters["@codigoBarra"].Value = CodigoBarra;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count > 0)
					{
						lista = dataSet.Tables[0].AsEnumerable().Select(dataRow => new LogObservaciones
						{
							Orden = dataRow.Field<string>("Orden")!,
							ObservacionOrden = dataRow.Field<string>("ObservacionOrden")!,
							Muestra = dataRow.Field<string>("Muestra")!,
							ObservacionMuestra = dataRow.Field<string>("ObservacionMuestra")!,
							Usuario = dataRow.Field<string>("Usuario")!,
							Fecha = dataRow.Field<DateTime>("Fecha")

						}).ToList();

						return lista;
					}
					else { return lista; }
				}
			}
		}
	}

	private List<ListarOrden> ListarOrdenes(ConsultarOrden valor)
	{
		List<ListarOrden> lista = new List<ListarOrden>();
		
		string FechaInicial = valor.FechaInicio.Replace("d", "/");
		string FechaFinal = valor.FechaFin.Replace("d", "/");

		DateTime FechaI = DateTime.ParseExact(FechaInicial, "dd/MM/yyyy", CultureInfo.InvariantCulture);
		DateTime FechaF = DateTime.ParseExact(FechaFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture);

		string fechaFormateadaI = FechaI.ToString("yyyy-MM-dd");
		string fechaFormateadaF = FechaF.ToString("yyyy-MM-dd");

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureListarOrdenes, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@idUsuarioGalileo", SqlDbType.VarChar);
				command.Parameters["@idUsuarioGalileo"].Value = valor.IdUsuarioGalileo;

				command.Parameters.Add("@opcionBusqueda", SqlDbType.Int);
				command.Parameters["@opcionBusqueda"].Value = valor.OpcionBusqueda;

				command.Parameters.Add("@opcionEstado", SqlDbType.VarChar);
				command.Parameters["@opcionEstado"].Value = valor.opcionEstado;

				command.Parameters.Add("@datoBusqueda", SqlDbType.VarChar);
				command.Parameters["@datoBusqueda"].Value = valor.DatoBusqueda;

				command.Parameters.Add("@idOrden", SqlDbType.Int);
				command.Parameters["@idOrden"].Value = valor.IdOrden;

				command.Parameters.Add("@codigoBarra", SqlDbType.VarChar);
				command.Parameters["@codigoBarra"].Value = valor.CodigoBarra;

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
						lista = dataSet.Tables[0].AsEnumerable().Select(dataRow => new ListarOrden
						{

							NOrden = dataRow.Field<int>("NOrden"),
							CodigoBarra = dataRow.Field<string>("CodigoBarra")!,
							FechaIngreso = dataRow.Field<DateTime>("FechaIngreso"),
							NombrePaciente = dataRow.Field<string>("NombrePaciente")!,
							Precio = ConvertDecimalToFloat(dataRow.Field<decimal?>("Precio")),
							Estado = dataRow.Field<string>("Estado")!,
							Observacion = dataRow.Field<string>("Observacion")!,
							CodigoGalileo = string.IsNullOrEmpty(dataRow.Field<string>("CodigoGalileo")) ? "-" : dataRow.Field<string>("CodigoGalileo")!

						}).ToList();

						return lista;
					}
					else { return lista; }
				}
			}
		}
	}

	private List<Muestras> ListarMuestras(int IdOrden)
	{
		List<Muestras> lista = new List<Muestras>();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureConsultarOrden, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C3";

				command.Parameters.Add("@idOrden", SqlDbType.Int);
				command.Parameters["@idOrden"].Value = IdOrden;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if(dataSet.Tables.Count > 0)
					{
						lista = dataSet.Tables[0].AsEnumerable().Select(dataRow => new Muestras
						{

							IdMuestra = dataRow.Field<int>("IdMuestra"),
							CodigoBarra = dataRow.Field<string>("CodigoBarra")!,
							UsuarioCreacion = dataRow.Field<int>("UsuarioCreacion")

						}).ToList();

						return lista;
					}
					else
					{
						return lista;
					}
				}
			}
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

	//verificar despues GrabarPaciente
	public void GrabarPaciente(Pacientes request)
	{
		int genero = Numeros.Cero;

		if (request.Genero == true)
		{
			genero = Numeros.Uno;
		}
		else if (request.Genero == false)
		{
			genero = Numeros.Cero;
		}

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedurePaciente, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "I";

				command.Parameters.Add("@identificacion", SqlDbType.VarChar);
				command.Parameters["@identificacion"].Value = request.Identificacion;

				command.Parameters.Add("@nombres", SqlDbType.VarChar);
				command.Parameters["@nombres"].Value = request.Nombres;

				command.Parameters.Add("@apellidos", SqlDbType.VarChar);
				command.Parameters["@apellidos"].Value = request.Apellidos;

				command.Parameters.Add("@genero", SqlDbType.Bit);
				command.Parameters["@genero"].Value = genero;

				command.Parameters.Add("@fechaNacimiento", SqlDbType.Date);
				command.Parameters["@fechaNacimiento"].Value = request.FechaNacimiento;

				command.Parameters.Add("@edad", SqlDbType.Int);
				command.Parameters["@edad"].Value = request.Edad;

				command.Parameters.Add("@telefono", SqlDbType.VarChar);
				command.Parameters["@telefono"].Value = request.Telefono;

				command.Parameters.Add("@email", SqlDbType.VarChar);
				command.Parameters["@email"].Value = request.Email;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = request.UsuarioCreacion;

				command.Parameters.Add("@fechaCreacion", SqlDbType.DateTime);
				command.Parameters["@fechaCreacion"].Value = request.FechaCreacion;

				command.Parameters.Add("@tipoPaciente", SqlDbType.Int);
				command.Parameters["@tipoPaciente"].Value = request.TipoPaciente;

				command.Parameters.Add("@codLab", SqlDbType.VarChar);
				command.Parameters["@codLab"].Value = request.CodLaboratorio;

				connection.Open();
				command.ExecuteNonQuery();
			}
		}		
	}

	private int GrabarOrden(GrabarOrdenRequest request)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureNuevaOrden, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "I";

				command.Parameters.Add("@idOrden", SqlDbType.Int);
				command.Parameters["@idOrden"].Value = request.IdOrden;

				command.Parameters.Add("@idPedido", SqlDbType.Int);
				command.Parameters["@idPedido"].Value = request.IdPedido;

				command.Parameters.Add("@idUsuarioGalileo", SqlDbType.Int);
				command.Parameters["@idUsuarioGalileo"].Value = request.IdUsuarioGalileo;

				command.Parameters.Add("@identificacionPaciente", SqlDbType.VarChar);
				command.Parameters["@identificacionPaciente"].Value = request.Identificacion;

				command.Parameters.Add("@codigoBarra", SqlDbType.VarChar);
				command.Parameters["@codigoBarra"].Value = request.CodigoBarra;

				command.Parameters.Add("@medico", SqlDbType.VarChar);
				command.Parameters["@medico"].Value = request.Medicamento;

				command.Parameters.Add("@diagnostico", SqlDbType.VarChar);
				command.Parameters["@diagnostico"].Value = request.Diagnostico;

				command.Parameters.Add("@observacion", SqlDbType.VarChar);
				command.Parameters["@observacion"].Value = request.Observacion;

				command.Parameters.Add("@estado", SqlDbType.Int);
				command.Parameters["@estado"].Value = request.Estado;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = request.UsuarioCreacion;

				command.Parameters.Add("@fechaCreacion", SqlDbType.Date);
				command.Parameters["@fechaCreacion"].Value = request.FechaCreacion;

				command.Parameters.Add("@empresaId", SqlDbType.Int);
				command.Parameters["@empresaId"].Value = request.EmpresaId;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						return _ = Convert.ToInt32(dataSet.Tables[0].Rows[0]["IdOrden"]);

					}
					else { return 0; }
				}
			}
		}		
	}

	private int GrabarPrueba(Pruebas request)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedurePrueba, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "I";

				command.Parameters.Add("@idPruebaGalileo", SqlDbType.Int);
				command.Parameters["@idPruebaGalileo"].Value = request.IdPruebaGalileo;

				command.Parameters.Add("@codigoBarra", SqlDbType.VarChar);
				command.Parameters["@codigoBarra"].Value = request.CodigoBarra;

				command.Parameters.Add("@esPerfil", SqlDbType.Bit);
				command.Parameters["@esPerfil"].Value = request.EsPerfil;

				command.Parameters.Add("@codigoExamen", SqlDbType.VarChar);
				command.Parameters["@codigoExamen"].Value = request.CodigoExamen;

				command.Parameters.Add("@nombre", SqlDbType.VarChar);
				command.Parameters["@nombre"].Value = request.Nombre;

				command.Parameters.Add("@abreviatura", SqlDbType.VarChar);
				command.Parameters["@abreviatura"].Value = request.Abreviatura;

				command.Parameters.Add("@metodologia", SqlDbType.VarChar);
				command.Parameters["@metodologia"].Value = request.Metodologia;

				command.Parameters.Add("@precio", SqlDbType.Money);
				command.Parameters["@precio"].Value = request.Precio;

				command.Parameters.Add("@estado", SqlDbType.Int);
				command.Parameters["@estado"].Value = request.EstadoPrueba;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = request.UsuarioCreacion;

				command.Parameters.Add("@FechaCreacion", SqlDbType.DateTime);
				command.Parameters["@FechaCreacion"].Value = request.FechaCreacion;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					var NumeroPrueba = Convert.ToInt32(dataSet.Tables[0].Rows[0]["IdPrueba"]);

					return NumeroPrueba;
				}
			}
		}		
	}

	private int ExisteMuestra(int orden, int muestra)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureMuestra, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@idOrden", SqlDbType.VarChar);
				command.Parameters["@idOrden"].Value = orden;

				command.Parameters.Add("@idMuestraGalileo", SqlDbType.VarChar);
				command.Parameters["@idMuestraGalileo"].Value = muestra;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						return _ = Convert.ToInt32(dataSet.Tables[0].Rows[0]["IdMuestra"]);

					}
					else { return 0; }
				}
			}
		}		
	}

	private int GrabarMuestra(int MuestraGalileo, int IdOrden, string Nombre, string MuestraAlterna, string Recipiente, string CodigoBarra, int Estado, int Usuario, DateTime Fecha)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureMuestra, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "I";

				command.Parameters.Add("@idOrden", SqlDbType.Int);
				command.Parameters["@idOrden"].Value = IdOrden;

				command.Parameters.Add("@idMuestraGalileo", SqlDbType.Int);
				command.Parameters["@idMuestraGalileo"].Value = MuestraGalileo;

				command.Parameters.Add("@nombre", SqlDbType.VarChar);
				command.Parameters["@nombre"].Value = Nombre;

				command.Parameters.Add("@muestraAlterna", SqlDbType.VarChar);
				command.Parameters["@muestraAlterna"].Value = MuestraAlterna;

				command.Parameters.Add("@recipiente", SqlDbType.VarChar);
				command.Parameters["@recipiente"].Value = Recipiente;

				command.Parameters.Add("@codigoBarra", SqlDbType.VarChar);
				command.Parameters["@codigoBarra"].Value = CodigoBarra;

				command.Parameters.Add("@estadoMuestra", SqlDbType.Int);
				command.Parameters["@estadoMuestra"].Value = Estado;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = Usuario;

				command.Parameters.Add("@fechaCreacion", SqlDbType.Date);
				command.Parameters["@fechaCreacion"].Value = Fecha;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					var NumeroMuestra = Convert.ToInt32(dataSet.Tables[0].Rows[0]["IdMuestra"]);
					return NumeroMuestra;
				}
			}
		}
	}

	//Verificar despues
	private void GrabarPruebaMuestra(int CodigoPrueba, int CodigoMuestra, int UsuarioCreacion, DateTime FechaCreacion)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedurePruebaMuestra, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "I";

				command.Parameters.Add("@idPrueba", SqlDbType.Int);
				command.Parameters["@idPrueba"].Value = CodigoPrueba;

				command.Parameters.Add("@idMuestra", SqlDbType.Int);
				command.Parameters["@idMuestra"].Value = CodigoMuestra;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = UsuarioCreacion;

				command.Parameters.Add("@fechaCreacion", SqlDbType.Date);
				command.Parameters["@fechaCreacion"].Value = FechaCreacion;

				connection.Open();
				command.ExecuteNonQuery();
			}
		}
	}

	private string ObtenerValEstaOrdenPrueba(int idEstado, char accion)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureEstaOrdPrue, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = accion;

				command.Parameters.Add("@i_idEstado", SqlDbType.Int);
				command.Parameters["@i_idEstado"].Value = idEstado;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					var valor = Convert.ToString(dataSet.Tables[0].Rows[0]["Column1"]);
					return valor!;
				}
			}
		}		
	}

	//verificar despues
	public void ActualizarPaciente(Pacientes request)
	{
		int genero = Numeros.Cero;

		if (request.Genero == true)
		{
			genero = Numeros.Uno;
		}
		else if (request.Genero == false)
		{
			genero = Numeros.Cero;
		}

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedurePaciente, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "M";

				command.Parameters.Add("@identificacion", SqlDbType.VarChar);
				command.Parameters["@identificacion"].Value = request.Identificacion;

				command.Parameters.Add("@nombres", SqlDbType.VarChar);
				command.Parameters["@nombres"].Value = request.Nombres;

				command.Parameters.Add("@apellidos", SqlDbType.VarChar);
				command.Parameters["@apellidos"].Value = request.Apellidos;

				command.Parameters.Add("@genero", SqlDbType.Bit);
				command.Parameters["@genero"].Value = genero;

				command.Parameters.Add("@fechaNacimiento", SqlDbType.Date);
				command.Parameters["@fechaNacimiento"].Value = request.FechaNacimiento;

				command.Parameters.Add("@edad", SqlDbType.Int);
				command.Parameters["@edad"].Value = request.Edad;

				command.Parameters.Add("@telefono", SqlDbType.VarChar);
				command.Parameters["@telefono"].Value = request.Telefono;

				command.Parameters.Add("@email", SqlDbType.VarChar);
				command.Parameters["@email"].Value = request.Email;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = request.UsuarioCreacion;

				command.Parameters.Add("@fechaCreacion", SqlDbType.DateTime);
				command.Parameters["@fechaCreacion"].Value = request.FechaCreacion;

				command.Parameters.Add("@tipoPaciente", SqlDbType.Int);
				command.Parameters["@tipoPaciente"].Value = request.TipoPaciente;

				command.Parameters.Add("@codLab", SqlDbType.VarChar);
				command.Parameters["@codLab"].Value = request.CodLaboratorio;

				connection.Open();
				command.ExecuteNonQuery();
			}
		}																									
	}

	private int ObtenerIdOrden(string codigoBarra)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureIdOrden, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@codigoBarra", SqlDbType.VarChar);
				command.Parameters["@codigoBarra"].Value = codigoBarra;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count == 0) return 0;
					if (dataSet.Tables[0].Rows.Count == 0) return 0;

					var NumeroOrden = Convert.ToInt32(dataSet.Tables[0].Rows[0]["IdOrden"]);

					// Se devuelve el objeto
					return NumeroOrden;
				}
			}
		}
	}

	//verficar despues
	private void ActualizarOrden(GrabarOrdenRequest request)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureNuevaOrden, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "M";

				command.Parameters.Add("@idOrden", SqlDbType.Int);
				command.Parameters["@idOrden"].Value = request.IdOrden;

				command.Parameters.Add("@idPedido", SqlDbType.Int);
				command.Parameters["@idPedido"].Value = request.IdPedido;

				command.Parameters.Add("@idUsuarioGalileo", SqlDbType.Int);
				command.Parameters["@idUsuarioGalileo"].Value = request.IdUsuarioGalileo;

				command.Parameters.Add("@identificacionPaciente", SqlDbType.VarChar);
				command.Parameters["@identificacionPaciente"].Value = request.Identificacion;

				command.Parameters.Add("@codigoBarra", SqlDbType.VarChar);
				command.Parameters["@codigoBarra"].Value = request.CodigoBarra;

				command.Parameters.Add("@medico", SqlDbType.VarChar);
				command.Parameters["@medico"].Value = request.Medicamento;

				command.Parameters.Add("@diagnostico", SqlDbType.VarChar);
				command.Parameters["@diagnostico"].Value = request.Diagnostico;

				command.Parameters.Add("@observacion", SqlDbType.VarChar);
				command.Parameters["@observacion"].Value = request.Observacion;

				command.Parameters.Add("@estado", SqlDbType.Int);
				command.Parameters["@estado"].Value = request.Estado;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = request.UsuarioCreacion;

				command.Parameters.Add("@fechaCreacion", SqlDbType.Date);
				command.Parameters["@fechaCreacion"].Value = request.FechaCreacion;

				connection.Open();
				command.ExecuteNonQuery();
			}
		}		
	}

	private int CantidadMuestra(int IdOrden)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureMuestra, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C1";

				command.Parameters.Add("@idOrden", SqlDbType.VarChar);
				command.Parameters["@idOrden"].Value = IdOrden;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					// Se realiza la consulta a la base de datos
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						return _ = Convert.ToInt32(dataSet.Tables[0].Rows[0]["Cantidad"]);
					}
					else { return 0; }
				}
			}
		}
	}

	private int ExistePruebas(int orden, int prueba)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureExistePrueba, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@idOrden", SqlDbType.VarChar);
				command.Parameters["@idOrden"].Value = orden;

				command.Parameters.Add("@idPruebaGalileo", SqlDbType.VarChar);
				command.Parameters["@idPruebaGalileo"].Value = prueba;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						return _ = Convert.ToInt32(dataSet.Tables[0].Rows[0]["Existe"]);
					}
					else { return 0; }
				}
			}
		}		
	}

	//verificar despues
	private void EliminarOrden(GrabarOrdenRequest request)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureNuevaOrden, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "E";

				command.Parameters.Add("@idOrden", SqlDbType.Int);
				command.Parameters["@idOrden"].Value = request.IdOrden;

				command.Parameters.Add("@estado", SqlDbType.Int);
				command.Parameters["@estado"].Value = request.Estado;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = request.UsuarioCreacion;

				command.Parameters.Add("@fechaCreacion", SqlDbType.Date);
				command.Parameters["@fechaCreacion"].Value = request.FechaCreacion;

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
	private void EliminarPrueba(int IdPrueba, int Estado, int UsuarioCreacion)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedurePrueba, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "E";

				command.Parameters.Add("@idPrueba", SqlDbType.Int);
				command.Parameters["@idPrueba"].Value = IdPrueba;

				command.Parameters.Add("@estado", SqlDbType.Int);
				command.Parameters["@estado"].Value = Estado;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = UsuarioCreacion;

				command.Parameters.Add("@FechaCreacion", SqlDbType.DateTime);
				command.Parameters["@FechaCreacion"].Value = DateTime.Now;

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
	private void EliminarPruebaMuestra(int IdMuestra, int IdPrueba, int UsuarioCreacion)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedurePruebaMuestra, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "E";

				command.Parameters.Add("@idMuestra", SqlDbType.Int);
				command.Parameters["@idMuestra"].Value = IdMuestra;

				command.Parameters.Add("@idPrueba", SqlDbType.Int);
				command.Parameters["@idPrueba"].Value = IdPrueba;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = UsuarioCreacion;

				command.Parameters.Add("@FechaCreacion", SqlDbType.DateTime);
				command.Parameters["@FechaCreacion"].Value = DateTime.Now;

				connection.Open();
				command.ExecuteNonQuery();
			}
		}		
	}

	//verificar despues
	private void EliminarMuestra(int IdMuestra, int IdOrden, int Estado, int UsuarioCreacion)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureMuestra, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "E";

				command.Parameters.Add("@idOrden", SqlDbType.Int);
				command.Parameters["@idOrden"].Value = IdOrden;

				command.Parameters.Add("@idMuestra2", SqlDbType.Int);
				command.Parameters["@idMuestra2"].Value = IdMuestra;

				command.Parameters.Add("@estadoMuestra", SqlDbType.Int);
				command.Parameters["@estadoMuestra"].Value = Estado;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = UsuarioCreacion;

				command.Parameters.Add("@FechaCreacion", SqlDbType.DateTime);
				command.Parameters["@FechaCreacion"].Value = DateTime.Now;

				connection.Open();
				command.ExecuteNonQuery();
			}
		}						
	}

	private int GetIdPrueba(int IdOrden, int IdPruebaGalileo)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureExistePrueba, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C1";

				command.Parameters.Add("@idOrden", SqlDbType.Int);
				command.Parameters["@idOrden"].Value = IdOrden;

				command.Parameters.Add("@idPruebaGalileo", SqlDbType.Int);
				command.Parameters["@idPruebaGalileo"].Value = IdPruebaGalileo;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count == 0) return 0;
					if (dataSet.Tables[0].Rows.Count == 0) return 0;

					var NumeroOrden = Convert.ToInt32(dataSet.Tables[0].Rows[0]["IdPrueba"]);

					// Se devuelve el objeto
					return NumeroOrden;
				}
			}
		}		
	}

	private int GetIdPruebaMuestra(int IdPrueba)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureExistePrueba, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C2";

				command.Parameters.Add("@idPrueba", SqlDbType.Int);
				command.Parameters["@idPrueba"].Value = IdPrueba;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count == 0) return 0;
					if (dataSet.Tables[0].Rows.Count == 0) return 0;

					var NumeroOrden = Convert.ToInt32(dataSet.Tables[0].Rows[0]["IdPruebaMuestra"]);

					// Se devuelve el objeto
					return NumeroOrden;
				}
			}
		}
	}

	private int GetIdMuestra(int IdPruebaMuestra)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureExistePrueba, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C3";

				command.Parameters.Add("@idPruebaMuestra", SqlDbType.Int);
				command.Parameters["@idPruebaMuestra"].Value = IdPruebaMuestra;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count == 0) return 0;
					if (dataSet.Tables[0].Rows.Count == 0) return 0;

					var NumeroOrden = Convert.ToInt32(dataSet.Tables[0].Rows[0]["IdMuestra"]);

					// Se devuelve el objeto
					return NumeroOrden;
				}
			}
		}
	}

	private int ContarPruebaMuestra(int IdMuestra)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureExistePrueba, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C4";

				command.Parameters.Add("@idMuestra", SqlDbType.Int);
				command.Parameters["@idMuestra"].Value = IdMuestra;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count == 0) return 0;
					if (dataSet.Tables[0].Rows.Count == 0) return 0;

					var NumeroOrden = Convert.ToInt32(dataSet.Tables[0].Rows[0]["Cantidad"]);

					// Se devuelve el objeto
					return NumeroOrden;
				}
			}
		}
	}

	private int GuardarResultados(string CodigoBarra, string Resultados, int Estado, int UsuarioCreacion)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureNuevaOrden, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "M2";

				command.Parameters.Add("@codigoBarra", SqlDbType.VarChar);
				command.Parameters["@codigoBarra"].Value = CodigoBarra;

				command.Parameters.Add("@estado", SqlDbType.Int);
				command.Parameters["@estado"].Value = Estado;

				command.Parameters.Add("@resultados", SqlDbType.Xml);
				command.Parameters["@resultados"].Value = Resultados;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = UsuarioCreacion;

				command.Parameters.Add("@fechaCreacion", SqlDbType.Date);
				command.Parameters["@fechaCreacion"].Value = DateTime.Now;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					var IdOrden = Convert.ToInt32(dataSet.Tables[0].Rows[0]["IdOrden"]);
					return IdOrden;
				}
			}
		}										
	}

	private List<Pruebas> ListPruebas(int IdOrden)
	{
		List<Pruebas> lista = new List<Pruebas>();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedurePrueba, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C2";

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
							IdPrueba = dataRow.Field<int>("IdPrueba"),
							EstadoPrueba = dataRow.Field<int>("Estado")

						}).ToList();

						return lista;
					}
					else { return lista; }
				}
			}
		}
	}

	//verificar despues
	private void ActualizarPruebasOrden(int IdPrueba, int Estado, int UsuarioCreacion)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureActualizacion, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "M";

				command.Parameters.Add("@idPrueba", SqlDbType.Int);
				command.Parameters["@idPrueba"].Value = IdPrueba;

				command.Parameters.Add("@estado", SqlDbType.Int);
				command.Parameters["@estado"].Value = Estado;

				command.Parameters.Add("@usuarioCreacion", SqlDbType.Int);
				command.Parameters["@usuarioCreacion"].Value = UsuarioCreacion;

				command.Parameters.Add("@fechaCreacion", SqlDbType.Date);
				command.Parameters["@fechaCreacion"].Value = DateTime.Now;

				connection.Open();
				command.ExecuteNonQuery();
			}
		}									
	}

	private string Base64Resultados(string CodigoBarra)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureOrdCodLis, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@codigoBarra", SqlDbType.VarChar);
				command.Parameters["@codigoBarra"].Value = CodigoBarra;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					string result = Convert.ToString(dataSet.Tables[0].Rows[0]["Column1"])!;
					return result!;
				}
			}
		}
	}

	public string NombreEstadoOrden(string idOrdenEstado)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureEstadoNombre, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@i_idEstado", SqlDbType.Int);
				command.Parameters["@i_idEstado"].Value = idOrdenEstado;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					var mensaje = Convert.ToString(dataSet.Tables[0].Rows[0]["Column1"]);
					return mensaje!;
				}
			}
		}
	}

	public string OrdenActuaPdfResult(ActualizaPdfOrden actualizaPdfOrden)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureActuOrdPdf, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "M";

				command.Parameters.Add("@i_idEstado", SqlDbType.Int);
				command.Parameters["@i_idEstado"].Value = actualizaPdfOrden.idEstado;

				command.Parameters.Add("@codigoBarra", SqlDbType.VarChar);
				command.Parameters["@codigoBarra"].Value = actualizaPdfOrden.codBarra;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					var mensaje = Convert.ToString(dataSet.Tables[0].Rows[0]["resultado"]);
					return mensaje!;
				}
			}
		}		
	}

	private int ObtenerIdClienteOrden(int idGalileo, string accion)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureEstaOrdPrue, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = accion;

				command.Parameters.Add("@i_idGalileo", SqlDbType.Int);
				command.Parameters["@i_idGalileo"].Value = idGalileo;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					var valor = Convert.ToInt32(dataSet.Tables[0].Rows[0]["Column1"]);
					return valor;
				}
			}
		}
	}

	private int ObtenerIdClientePedido(int id)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureClienteIdPed, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@i_idCliente", SqlDbType.Int);
				command.Parameters["@i_idCliente"].Value = id;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					var valor = Convert.ToInt32(dataSet.Tables[0].Rows[0]["IdCliente"]);
					return valor;
				}
			}
		}
	}

	private string OrdenActuaPdfResultFinal(string codOrdenHuma, string codOrdenLis,
					int idLab, string pdfBase64, int idOrdenValor)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureActuOrdPdf, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "P";

				command.Parameters.Add("@codigoBarra", SqlDbType.VarChar);
				command.Parameters["@codigoBarra"].Value = codOrdenLis;

				command.Parameters.Add("@codigoBarraHuma", SqlDbType.VarChar);
				command.Parameters["@codigoBarraHuma"].Value = codOrdenHuma;

				command.Parameters.Add("@idLab", SqlDbType.Int);
				command.Parameters["@idLab"].Value = idLab;

				command.Parameters.Add("@basePdf", SqlDbType.Xml);
				command.Parameters["@basePdf"].Value = pdfBase64;

				command.Parameters.Add("@i_idEstado", SqlDbType.Int);
				command.Parameters["@i_idEstado"].Value = idOrdenValor;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					var mensaje = Convert.ToString(dataSet.Tables[0].Rows[0]["resultado"]);
					return mensaje!;
				}
			}
		}
	}

	private string ConsultarBaseResultados(string codOrdenHuma)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureActuOrdPdf, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@codigoBarraHuma", SqlDbType.VarChar);
				command.Parameters["@codigoBarraHuma"].Value = codOrdenHuma;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					var mensaje = Convert.ToString(dataSet.Tables[0].Rows[0]["resultado"]);
					return mensaje!;
				}
			}
		}
	}

	private string ResultadosInsertar(ResultadosRequest resultadosRequest, string Informe)
	{
		string mensaje = "";

		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(StringHandler.ProcedureResultadosLab, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					//Envio de parametros
					command.Parameters.Add("@i_accion", SqlDbType.Char);
					command.Parameters["@i_accion"].Value = "I";

					command.Parameters.Add("@i_idLab", SqlDbType.Int);
					command.Parameters["@i_idLab"].Value = resultadosRequest.idLaboratorio;

					command.Parameters.Add("@i_idenpac", SqlDbType.VarChar);
					command.Parameters["@i_idenpac"].Value = resultadosRequest.Identificacion;

					command.Parameters.Add("@i_numOrden", SqlDbType.VarChar);
					command.Parameters["@i_numOrden"].Value = resultadosRequest.NumeroOrden;

					command.Parameters.Add("@i_informe", SqlDbType.Xml);
					command.Parameters["@i_informe"].Value = Informe;

					command.Parameters.Add("@i_estado", SqlDbType.VarChar);
					command.Parameters["@i_estado"].Value = resultadosRequest.Estado;

					command.Parameters.Add("@i_genero", SqlDbType.VarChar);
					command.Parameters["@i_genero"].Value = resultadosRequest.Genero;

					command.Parameters.Add("@i_fechaNacimiento", SqlDbType.VarChar);
					command.Parameters["@i_fechaNacimiento"].Value = resultadosRequest.FechaNacimiento;

					command.Parameters.Add("@i_fechaIngrenso", SqlDbType.VarChar);
					command.Parameters["@i_fechaIngrenso"].Value = resultadosRequest.FechaIngreso;

					command.Parameters.Add("@i_usuario", SqlDbType.VarChar);
					command.Parameters["@i_usuario"].Value = resultadosRequest.Usuario;

					command.Parameters.Add("@i_nombrePaciente", SqlDbType.VarChar);
					command.Parameters["@i_nombrePaciente"].Value = resultadosRequest.NombrePaciente;

					command.Parameters.Add("@i_idSede", SqlDbType.VarChar);
					command.Parameters["@i_idSede"].Value = resultadosRequest.idSede;

					command.Parameters.Add("@i_nombreSede", SqlDbType.VarChar);
					command.Parameters["@i_nombreSede"].Value = resultadosRequest.nombreSede;

					connection.Open();

					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataSet dataSet = new DataSet();
						adapter.Fill(dataSet);

						mensaje = Convert.ToString(dataSet.Tables[0].Rows[0]["Resultado"])!;
					}
				}
			}
		}
		catch(Exception ex)
		{
			return "01";
		}
		return mensaje!;
	}

	private List<ResultadosMedico> ListaResultadosMedicoLab(ConsultarResultados consultar)
	{
		List<ResultadosMedico> lista = new List<ResultadosMedico>();

		try
		{
			string FechaInicial = consultar.FechaInicio.Replace("d", "/");
			string FechaFinal = consultar.FechaFin.Replace("d", "/");

			DateTime FechaI = DateTime.ParseExact(FechaInicial, "dd/MM/yyyy", CultureInfo.InvariantCulture);
			DateTime FechaF = DateTime.ParseExact(FechaFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture);

			string fechaFormateadaI = FechaI.ToString("yyyy-MM-dd");
			string fechaFormateadaF = FechaF.ToString("yyyy-MM-dd");

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(StringHandler.ProcedureListaResultMedico, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					//Envio de parametros
					command.Parameters.Add("@i_accion", SqlDbType.Char);
					command.Parameters["@i_accion"].Value = "C";

					command.Parameters.Add("@opcionBusqueda", SqlDbType.Int);
					command.Parameters["@opcionBusqueda"].Value = consultar.OpcionBusqueda;

					command.Parameters.Add("@opcionEstado", SqlDbType.Int);
					command.Parameters["@opcionEstado"].Value = consultar.opcionEstado;

					command.Parameters.Add("@datoBusqueda", SqlDbType.VarChar);
					command.Parameters["@datoBusqueda"].Value = consultar.DatoBusqueda;

					command.Parameters.Add("@codigoBarra", SqlDbType.VarChar);
					command.Parameters["@codigoBarra"].Value = consultar.CodigoBarra;

					command.Parameters.Add("@fechaInicio", SqlDbType.Date);
					command.Parameters["@fechaInicio"].Value = fechaFormateadaI.ToString();

					command.Parameters.Add("@fechaFin", SqlDbType.Date);
					command.Parameters["@fechaFin"].Value = fechaFormateadaF.ToString();

					command.Parameters.Add("@idLab", SqlDbType.Int);
					command.Parameters["@idLab"].Value = consultar.idLaboratorio;

					command.Parameters.Add("@sedes", SqlDbType.VarChar);
					command.Parameters["@sedes"].Value = consultar.Sedes;

					connection.Open();

					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataSet dataSet = new DataSet();
						adapter.Fill(dataSet);

						if (dataSet.Tables.Count > 0)
						{
							lista = dataSet.Tables[0].AsEnumerable().Select(dataRow => new ResultadosMedico
							{
								idResultados = dataRow.Field<int>("idResultados"),
								Identificacion = dataRow.Field<string>("Identificacion")!,
								NumeroOrden = dataRow.Field<string>("NumeroOrden")!,
								Genero = dataRow.Field<string>("Genero")!,
								Estado = dataRow.Field<string>("Estado")!,
								FechaIngreso = Convert.ToString(dataRow.Field<string>("FechaIngreso"))!,
								NombrePaciente = dataRow.Field<string>("NombrePaciente")!,
								IdSede = dataRow.Field<string>("IdSede") != null ? dataRow.Field<string>("IdSede")! : "-",
								NombreSede = dataRow.Field<string>("NombreSede") != null ? dataRow.Field<string>("NombreSede")! : "-"

							}).ToList();

							return lista;
						}
						else
						{
							return lista;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			return lista;
		}
	}

	private string PdfFinalResultMedico(string codBarra, int idResult)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureListaResultMedico, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "P";

				command.Parameters.Add("@codigoBarra", SqlDbType.VarChar);
				command.Parameters["@codigoBarra"].Value = codBarra;

				command.Parameters.Add("@idResult", SqlDbType.Int);
				command.Parameters["@idResult"].Value = idResult;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					var mensaje = Convert.ToString(dataSet.Tables[0].Rows[0]["Resultado"]);
					return mensaje!;
				}
			}
		}						
	}

	private string ResultPdfNew(int idLab, string codOrden)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedurePdfResultNew, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@i_idLab", SqlDbType.Int);
				command.Parameters["@i_idLab"].Value = idLab;

				command.Parameters.Add("@i_codOrden", SqlDbType.VarChar);
				command.Parameters["@i_codOrden"].Value = codOrden;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					var mensaje = Convert.ToString(dataSet.Tables[0].Rows[0]["Resultado"]);

					if (mensaje != "01")
					{
						return mensaje!;
					}
					else
					{
						return "Resultado no encontrado";
					}
				}
			}
		}							
	}

	private async Task<string> ConsultPacienteResult(string Identificacion, int idLab)
	{
		string jsonResultado = "";
		List<ResultPacienteResponse> lstResult = new();

		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(StringHandler.ProcedurePdfResultNew, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					//Envio de parametros
					command.Parameters.Add("@i_accion", SqlDbType.Char);
					command.Parameters["@i_accion"].Value = "P";

					command.Parameters.Add("@i_identificacion", SqlDbType.VarChar);
					command.Parameters["@i_identificacion"].Value = Identificacion;

					command.Parameters.Add("@i_idLab", SqlDbType.Int);
					command.Parameters["@i_idLab"].Value = idLab;

					await connection.OpenAsync();

					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataSet dataSet = new DataSet();
						adapter.Fill(dataSet);

						if (dataSet.Tables[0].Rows.Count > 0)
						{
							lstResult = ConvertToList<ResultPacienteResponse>(dataSet.Tables[0]);
							jsonResultado = ArmarJsonResult(lstResult);
						}
						else
						{
							return "No existen resultados del paciente";
						}

						return jsonResultado;
					}
				}
			}
		}
		catch (Exception ex)
		{
			return "No existen resultados del paciente";
		}
	}

	#region conversion de dataset
	//Conversiones dataset a una clase
	public static T ConvertTo<T>(DataTable dataTable) where T : new()
	{
		T obj = new T();
		foreach (DataRow row in dataTable.Rows)
		{
			foreach (DataColumn column in dataTable.Columns)
			{
				PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName)!;
				
				if (prop != null)
				{
					if (row[column] != DBNull.Value)
					{
						// Obtén el tipo de propiedad
						Type propertyType = prop.PropertyType;

						// Maneja los tipos anulables
						if (Nullable.GetUnderlyingType(propertyType) != null)
						{
							propertyType = Nullable.GetUnderlyingType(propertyType)!;
						}

						// Realiza la conversión y asigna el valor
						object value = Convert.ChangeType(row[column], propertyType);
						prop.SetValue(obj, value);
					}
					else
					{
						// Si el valor es DBNull, se asigna null para tipos anulables
						if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
						{
							prop.SetValue(obj, null);
						}
					}
				}

			}
		}
		return obj;
	}

	protected List<T> ConvertToList<T>(DataTable table) where T : new()
	{
		List<T> list = new List<T>();
		try
		{
			List<string> list2 = new List<string>();
			foreach (DataColumn column in table.Columns)
			{
				list2.Add(column.ColumnName);
			}

			foreach (DataRow row in table.Rows)
			{
				list.Add(GetObject<T>(row, list2));
			}

			return list;
		}
		catch
		{
			return list;
		}
	}

	private T GetObject<T>(DataRow row, List<string> columnNames) where T : new()
	{
		T val = new T();
		try
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			decimal result = default(decimal);
			PropertyInfo[] properties = typeof(T).GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				empty = columnNames.Find((string name) => name.ToLower() == propertyInfo.Name.ToLower())!;
				if (propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), inherit: true).Length != 0)
				{
					empty = ((ColumnAttribute)propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), inherit: true)[0]).Name!;
				}

				if (string.IsNullOrEmpty(empty))
				{
					continue;
				}

				empty2 = row[empty].ToString()!;
				if (string.IsNullOrEmpty(empty2))
				{
					continue;
				}

				if (!decimal.TryParse(empty2, out result))
				{
					if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null)
					{
						propertyInfo.SetValue(val, Convert.ChangeType(empty2, Type.GetType(Nullable.GetUnderlyingType(propertyInfo.PropertyType)!.ToString())), null);
					}
					else if (propertyInfo.PropertyType.IsEnum)
					{
						propertyInfo.SetValue(val, Enum.Parse(propertyInfo.PropertyType, empty2), null);
					}
					else
					{
						propertyInfo.SetValue(val, Convert.ChangeType(empty2, Type.GetType(propertyInfo.PropertyType.ToString())), null);
					}
				}
				else if (propertyInfo.PropertyType.IsEnum)
				{
					propertyInfo.SetValue(val, Enum.Parse(propertyInfo.PropertyType, empty2), null);
				}
				else
				{
					propertyInfo.SetValue(val, Convert.ChangeType(empty2, Type.GetType(propertyInfo.PropertyType.ToString())), null);
				}
			}

			return val;
		}
		catch
		{
			return val;
		}
	}

    private static float ConvertDecimalToFloat(decimal? value)
    {        
        if (!value.HasValue)
            return default;

        try
        {            
            return Convert.ToSingle(value.Value);
        }
        catch (OverflowException)
        {            
            return float.NaN;
        }
    }

    #endregion

    #region Armar JSON
    private string ArmarJsonResult(List<ResultPacienteResponse> lstResultPaciente)
	{
		var grupos = lstResultPaciente.GroupBy(r => new
		{
			r.Cedula,
			r.NombrePaciente,
			r.NumeroOrden,
			r.Estado,
			r.FechaIngreso,
			r.IdLaboratorio
		}).ToList();

		string jsonResult = JsonConvert.SerializeObject(grupos);
		return jsonResult;
	}

	#endregion
}
