using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using PedidosHL.Entidades.Consultas;
using PedidosHL.Entidades.Operaciones;
using PedidosHL.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace PedidosHL.Datos;

public interface IMapeoDatosPedido
{
	Task<List<BuscarPedidoOperadorResponse>> BuscarPedidoOperadorAsync(BuscarPedidoOperadorRequest request);
	Task<string> BuscarPedidoOperadorActuAsync(int idPedido, int idOperador);
	Task<VerDetallePedidoOperadorResponse> VerDetallePedidoOperadorAsync(VerDetallePedidoOperadorRequest request);
	Task<VerDetallePedidoOperadorResponse> VerDetallePedidoOperadorNewAsync(VerDetallePedidoOperadorRequest request);
	Task<RecogerMuestraResponse> RecogerMuestraAsync(RecogerMuestraRequest request);
	Task<RecogerPedidoResponse> RecogerPedidoAsync(RecogerPedidoRequest request);
	Task<EntregarPedidoResponse> EntregarPedidoAsync(EntregarPedidoRequest request);
	Task<BuscarOrdenLaboratoristaResponse> BuscarOrdenLaboratoristaAsync(BuscarOrdenLaboratoristaRequest request);
	Task<VerOrdenLaboratoristaResponse> VerOrdenLaboratoristaAsync(BuscarOrdenLaboratoristaRequest request);
	List<EstadosCatalogoResponse> ConsultarEstadosCatalogo(string nombreMaestro);
	Task<string> ConsultarOrdenGalileoAsync(int idOrden);
	Task<string> RecibirOrdenAsync(int idOrden, string codExterno);
	Task<CambiarPruebaResponse> CambiarPruebaAsync(CambiarPruebaRequest request);
	Task<List<PruebasResponse>> ConsultaPruebasAsync(int idMuestra);
	Task<string> ActualizaPedido(int idPedido);
}

public class MapeoDatosPedido: IMapeoDatosPedido
{
	private readonly string connectionString;

	#region constructor
	public MapeoDatosPedido()
	{
		connectionString = Environment.GetEnvironmentVariable(StringHandler.DATABASEDEV)!;

		if (string.IsNullOrEmpty(connectionString))
		{
			throw new InvalidOperationException("La cadena de conexión no está configurada en las variables de entorno.");
		}

		// Configura el SqlConnectionStringBuilder con la cadena de conexión obtenida
		SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
		connectionString = builder.ConnectionString;
	}

	#endregion

	public async Task<List<BuscarPedidoOperadorResponse>> BuscarPedidoOperadorAsync(BuscarPedidoOperadorRequest request)
	{
		return await BuscarPedidoOperador(request);
	}

	public async Task<string> BuscarPedidoOperadorActuAsync(int idPedido, int idOperador)
	{
		return await BuscarPedidoOperadorActua(idPedido, idOperador);
	}

	public async Task<VerDetallePedidoOperadorResponse> VerDetallePedidoOperadorAsync(VerDetallePedidoOperadorRequest request)
	{
		return await VerDetallePedidoOperador(request);
	}

	public async Task<VerDetallePedidoOperadorResponse> VerDetallePedidoOperadorNewAsync(VerDetallePedidoOperadorRequest request)
	{
		return await VerDetallePedidoOperadorNew(request);
	}

	public async Task<RecogerMuestraResponse> RecogerMuestraAsync(RecogerMuestraRequest request)
	{
		return await RecogerMuestra(request);
	}

	public async Task<RecogerPedidoResponse> RecogerPedidoAsync(RecogerPedidoRequest request)
	{
		return await RecogerPedidos(request);
	}

	public async Task<EntregarPedidoResponse> EntregarPedidoAsync(EntregarPedidoRequest request)
	{
		return await EntregarPedidos(request);
	}

	public async Task<BuscarOrdenLaboratoristaResponse> BuscarOrdenLaboratoristaAsync(BuscarOrdenLaboratoristaRequest request)
	{
		return await BuscarOrdenLaboratorista(request);
	}

	public async Task<VerOrdenLaboratoristaResponse> VerOrdenLaboratoristaAsync(BuscarOrdenLaboratoristaRequest request)
	{
		return await VerOrdenLaboratorista(request);
	}

	public List<EstadosCatalogoResponse> ConsultarEstadosCatalogo(string nombreMaestro)
	{
		return EstadosCatalogo(nombreMaestro);
	}

	public async Task<string> ConsultarOrdenGalileoAsync(int idOrden)
	{
		return await ConsultarOrdenGalileo(idOrden);
	}

	public async Task<string> RecibirOrdenAsync(int idOrden, string codExterno)
	{
		return await RecibirOrden(idOrden, codExterno);
	}

	public async Task<CambiarPruebaResponse> CambiarPruebaAsync(CambiarPruebaRequest request)
	{
		return await CambiarPrueba(request);
	}

	public async Task<List<PruebasResponse>> ConsultaPruebasAsync(int idMuestra)
	{
		return await ConsultaPruebas(idMuestra);
	}

	public async Task<string> ActualizaPedido(int idPedido)
	{
		return await ActualizaPedidoVerif(idPedido);
	}

	#region consultas BD

	public async Task<List<BuscarPedidoOperadorResponse>> BuscarPedidoOperador(BuscarPedidoOperadorRequest request)
	{
		List<BuscarPedidoOperadorResponse> data = new List<BuscarPedidoOperadorResponse>();

		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(StringHandler.ProcedureOperadorPedidos, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					//Envio de parametros
					command.Parameters.Add("@i_accion", SqlDbType.Char);
					command.Parameters["@i_accion"].Value = "C";

					command.Parameters.Add("@i_fecha_desde", SqlDbType.DateTime);
					command.Parameters["@i_fecha_desde"].Value = (object)request.FechaDesde! ?? DBNull.Value;

					command.Parameters.Add("@i_fecha_hasta", SqlDbType.DateTime);
					command.Parameters["@i_fecha_hasta"].Value = (object)request.FechaHasta! ?? DBNull.Value;

					command.Parameters.Add("@i_cliente", SqlDbType.VarChar);
					command.Parameters["@i_cliente"].Value = (object)request.Cliente! ?? DBNull.Value;

					command.Parameters.Add("@i_es_identificacion", SqlDbType.Bit);
					command.Parameters["@i_es_identificacion"].Value = (object)request.BuscarPorIdentificacion! ?? DBNull.Value;

					command.Parameters.Add("@i_operador_logistico", SqlDbType.Int);
					command.Parameters["@i_operador_logistico"].Value = (object)request.IdOperadorLogistico! ?? DBNull.Value;

					command.Parameters.Add("@i_estado", SqlDbType.VarChar);
					command.Parameters["@i_estado"].Value = (object)request.Estado! ?? DBNull.Value;

					command.Parameters.Add("@i_pedido", SqlDbType.Int);
					command.Parameters["@i_pedido"].Value = DBNull.Value;

					command.Parameters.Add("@i_observacion_operador", SqlDbType.VarChar);
					command.Parameters["@i_observacion_operador"].Value = DBNull.Value;

					command.Parameters.Add("@i_pedidos_entrega", SqlDbType.VarChar);
					command.Parameters["@i_pedidos_entrega"].Value = DBNull.Value;

					await connection.OpenAsync();

					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataSet dataSet = new DataSet();
						adapter.Fill(dataSet);

						if (dataSet.Tables[0].Rows.Count > 0)
							data = ConvertToList<BuscarPedidoOperadorResponse>(dataSet.Tables[0]);
						return data;
					}
				}
            }
		}
		catch (Exception ex)
		{
			BuscarPedidoOperadorResponse r = new BuscarPedidoOperadorResponse("001", ex.Message);
			List<BuscarPedidoOperadorResponse> resp = new List<BuscarPedidoOperadorResponse>();
			resp.Add(r);
			return resp;
		}
	}

	public async Task<string> BuscarPedidoOperadorActua(int idPedido, int idOperador)
	{
		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(StringHandler.ProcedureOperadorPedidosActu, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					//Envio de parametros
					command.Parameters.Add("@i_accion", SqlDbType.Char);
					command.Parameters["@i_accion"].Value = "M";

					command.Parameters.Add("@i_operador_logistico", SqlDbType.Int);
					command.Parameters["@i_operador_logistico"].Value = (object)idOperador ?? DBNull.Value;

					command.Parameters.Add("@i_idPedido", SqlDbType.Int);
					command.Parameters["@i_idPedido"].Value = (object)idPedido ?? DBNull.Value;

					await connection.OpenAsync();

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
		catch (Exception e)
		{
			var resp = "01";
			return resp;
		}
	}

	public async Task<VerDetallePedidoOperadorResponse> VerDetallePedidoOperador(VerDetallePedidoOperadorRequest request)
	{
		VerDetallePedidoOperadorResponse datos = new VerDetallePedidoOperadorResponse();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureOperadorPedidos, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "CD";

				command.Parameters.Add("@i_fecha_desde", SqlDbType.DateTime);
				command.Parameters["@i_fecha_desde"].Value = DBNull.Value;

				command.Parameters.Add("@i_fecha_hasta", SqlDbType.DateTime);
				command.Parameters["@i_fecha_hasta"].Value = DBNull.Value;

				command.Parameters.Add("@i_cliente", SqlDbType.VarChar);
				command.Parameters["@i_cliente"].Value = DBNull.Value;

				command.Parameters.Add("@i_es_identificacion", SqlDbType.Bit);
				command.Parameters["@i_es_identificacion"].Value = DBNull.Value;

				command.Parameters.Add("@i_operador_logistico", SqlDbType.Int);
				command.Parameters["@i_operador_logistico"].Value = DBNull.Value;

				command.Parameters.Add("@i_estado", SqlDbType.VarChar);
				command.Parameters["@i_estado"].Value = DBNull.Value;

				command.Parameters.Add("@i_pedido", SqlDbType.Int);
				command.Parameters["@i_pedido"].Value = request.IdPedido;

				command.Parameters.Add("@i_observacion_operador", SqlDbType.VarChar);
				command.Parameters["@i_observacion_operador"].Value = DBNull.Value;

				command.Parameters.Add("@i_pedidos_entrega", SqlDbType.VarChar);
				command.Parameters["@i_pedidos_entrega"].Value = DBNull.Value;

				 await connection.OpenAsync();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						datos.Pedido = ConvertTo<VerDetallePedidoOperadorCab>(dataSet.Tables[0]);
						datos.Muestras = ConvertToList<VerDetallePedidoOperadorDet>(dataSet.Tables[1]);
					}
					return datos;
				}
			}
		}		
	}

	public async Task<VerDetallePedidoOperadorResponse> VerDetallePedidoOperadorNew(VerDetallePedidoOperadorRequest request)
	{
		VerDetallePedidoOperadorResponse datos = new VerDetallePedidoOperadorResponse();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureOperadorPedidosNew, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "CD";

				command.Parameters.Add("@i_fecha_desde", SqlDbType.DateTime);
				command.Parameters["@i_fecha_desde"].Value = DBNull.Value;

				command.Parameters.Add("@i_fecha_hasta", SqlDbType.DateTime);
				command.Parameters["@i_fecha_hasta"].Value = DBNull.Value;

				command.Parameters.Add("@i_cliente", SqlDbType.VarChar);
				command.Parameters["@i_cliente"].Value = DBNull.Value;

				command.Parameters.Add("@i_es_identificacion", SqlDbType.Bit);
				command.Parameters["@i_es_identificacion"].Value = DBNull.Value;

				command.Parameters.Add("@i_operador_logistico", SqlDbType.Int);
				command.Parameters["@i_operador_logistico"].Value = DBNull.Value;

				command.Parameters.Add("@i_estado", SqlDbType.VarChar);
				command.Parameters["@i_estado"].Value = DBNull.Value;

				command.Parameters.Add("@i_pedido", SqlDbType.Int);
				command.Parameters["@i_pedido"].Value = request.IdPedido;

				command.Parameters.Add("@i_observacion_operador", SqlDbType.VarChar);
				command.Parameters["@i_observacion_operador"].Value = DBNull.Value;

				command.Parameters.Add("@i_pedidos_entrega", SqlDbType.VarChar);
				command.Parameters["@i_pedidos_entrega"].Value = DBNull.Value;

				await connection.OpenAsync();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						datos.Pedido = ConvertTo<VerDetallePedidoOperadorCab>(dataSet.Tables[0]);
						datos.Muestras = ConvertToList<VerDetallePedidoOperadorDet>(dataSet.Tables[1]);
					}
					return datos;
				}
			}
		}
	}


	public async Task<RecogerMuestraResponse> RecogerMuestra(RecogerMuestraRequest request)
	{
		RecogerMuestraResponse respuesta = new RecogerMuestraResponse();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureOperadorMuestras, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "M";

				command.Parameters.Add("@i_id_prueba_muestra", SqlDbType.Int);
				command.Parameters["@i_id_prueba_muestra"].Value = (object)request.IdMuestra ?? DBNull.Value;

				command.Parameters.Add("@i_recolecta", SqlDbType.Bit);
				command.Parameters["@i_recolecta"].Value = (object)request.Recolectar ?? DBNull.Value;

				command.Parameters.Add("@i_rechaza", SqlDbType.Bit);
				command.Parameters["@i_rechaza"].Value = (object)request.Rechazar ?? DBNull.Value;

				command.Parameters.Add("@i_es_operador", SqlDbType.Bit);
				command.Parameters["@i_es_operador"].Value = (object)request.EsOperador ?? DBNull.Value;

				command.Parameters.Add("@i_usuario_operador", SqlDbType.Int);
				command.Parameters["@i_usuario_operador"].Value = (object)request.IdOperador ?? DBNull.Value;

				command.Parameters.Add("@i_nombre_usuario", SqlDbType.VarChar);
				command.Parameters["@i_nombre_usuario"].Value = (object)request.NombreUsuario ?? DBNull.Value;

				command.Parameters.Add("@i_observacion", SqlDbType.VarChar);
				command.Parameters["@i_observacion"].Value = (object)request.Observacion ?? DBNull.Value;

				await connection.OpenAsync();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						respuesta.IdMuestra = Convert.ToInt32(dataSet.Tables[0].Rows[0][0]);
						respuesta.EstadoMuestra = dataSet.Tables[0].Rows[0][1].ToString()!;
						respuesta.Observacion = dataSet.Tables[0].Rows[0][2].ToString()!;

						return respuesta;
					}
					return new RecogerMuestraResponse(request.IdMuestra.ToString(), "Error al actualizar la muestra");
				}
			}
		}						
	}

	public async Task<RecogerPedidoResponse> RecogerPedidos(RecogerPedidoRequest request)
	{
		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(StringHandler.ProcedureOperadorPedidos, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					//Envio de parametros
					command.Parameters.Add("@i_accion", SqlDbType.Char);
					command.Parameters["@i_accion"].Value = "M";

					command.Parameters.Add("@i_fecha_desde", SqlDbType.DateTime);
					command.Parameters["@i_fecha_desde"].Value = (object)DBNull.Value;

					command.Parameters.Add("@i_fecha_hasta", SqlDbType.DateTime);
					command.Parameters["@i_fecha_hasta"].Value = (object)DBNull.Value;

					command.Parameters.Add("@i_cliente", SqlDbType.VarChar);
					command.Parameters["@i_cliente"].Value = (object)DBNull.Value;

					command.Parameters.Add("@i_es_identificacion", SqlDbType.Bit);
					command.Parameters["@i_es_identificacion"].Value = (object)DBNull.Value;

					command.Parameters.Add("@i_operador_logistico", SqlDbType.Int);
					command.Parameters["@i_operador_logistico"].Value = (object)request.IdOperador;

					command.Parameters.Add("@i_estado", SqlDbType.VarChar);
					command.Parameters["@i_estado"].Value = (object)DBNull.Value;

					command.Parameters.Add("@i_pedido", SqlDbType.Int);
					command.Parameters["@i_pedido"].Value = (object)request.IdPedido ?? DBNull.Value;

					command.Parameters.Add("@i_observacion_operador", SqlDbType.VarChar);
					command.Parameters["@i_observacion_operador"].Value = (object)request.ObservacionOperador ?? DBNull.Value;

					command.Parameters.Add("@i_pedidos_entrega", SqlDbType.VarChar);
					command.Parameters["@i_pedidos_entrega"].Value = (object)DBNull.Value;

					await connection.OpenAsync();

					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataSet dataSet = new DataSet();
						adapter.Fill(dataSet);

						RecogerPedidoResponse r = new RecogerPedidoResponse();
						r.IdPedido = (int)dataSet.Tables[0].Rows[0][0];
						return r;
					}
				}
			}			
		}
		catch (Exception ex)
		{
			RecogerPedidoResponse r = new RecogerPedidoResponse("001", ex.Message);
			return r;
		}
	}

	public async Task<EntregarPedidoResponse> EntregarPedidos(EntregarPedidoRequest request)
	{
		EntregarPedidoResponse r = new EntregarPedidoResponse();

		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(StringHandler.ProcedureOperadorPedidos, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					//Envio de parametros
					command.Parameters.Add("@i_accion", SqlDbType.Char);
					command.Parameters["@i_accion"].Value = "M";

					command.Parameters.Add("@i_fecha_desde", SqlDbType.DateTime);
					command.Parameters["@i_fecha_desde"].Value = (object)DBNull.Value;

					command.Parameters.Add("@i_fecha_hasta", SqlDbType.DateTime);
					command.Parameters["@i_fecha_hasta"].Value = (object)DBNull.Value;

					command.Parameters.Add("@i_cliente", SqlDbType.VarChar);
					command.Parameters["@i_cliente"].Value = (object)DBNull.Value;

					command.Parameters.Add("@i_es_identificacion", SqlDbType.Bit);
					command.Parameters["@i_es_identificacion"].Value = (object)DBNull.Value;

					command.Parameters.Add("@i_operador_logistico", SqlDbType.Int);
					command.Parameters["@i_operador_logistico"].Value = (object)request.IdOperador;

					command.Parameters.Add("@i_estado", SqlDbType.VarChar);
					command.Parameters["@i_estado"].Value = (object)DBNull.Value;

					command.Parameters.Add("@i_pedido", SqlDbType.Int);
					command.Parameters["@i_pedido"].Value = (object)DBNull.Value;

					command.Parameters.Add("@i_observacion_operador", SqlDbType.VarChar);
					command.Parameters["@i_observacion_operador"].Value = (object)request.ObservacionOperador ?? DBNull.Value;

					command.Parameters.Add("@i_pedidos_entrega", SqlDbType.VarChar);
					command.Parameters["@i_pedidos_entrega"].Value = (object)request.Pedidos ?? DBNull.Value;

					await connection.OpenAsync();

					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataSet dataSet = new DataSet();
						adapter.Fill(dataSet);

						r.Entregado = (bool)dataSet.Tables[0].Rows[0][0];
						return r;
					}
				}
			}			
		}
		catch (Exception ex)
		{
			r = new EntregarPedidoResponse("001", ex.Message);
			return r;
		}
	}

	public async Task<BuscarOrdenLaboratoristaResponse> BuscarOrdenLaboratorista(BuscarOrdenLaboratoristaRequest request)
	{
		BuscarOrdenLaboratoristaResponse datos = new BuscarOrdenLaboratoristaResponse();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureOrdenesLaboratorista, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@i_fecha_desde", SqlDbType.DateTime);
				command.Parameters["@i_fecha_desde"].Value = (object)request.FechaDesde ?? DBNull.Value;

				command.Parameters.Add("@i_fecha_hasta", SqlDbType.DateTime);
				command.Parameters["@i_fecha_hasta"].Value = (object)request.FechaHasta ?? DBNull.Value;

				command.Parameters.Add("@i_operador", SqlDbType.VarChar);
				command.Parameters["@i_operador"].Value = (object)request.Operador! ?? DBNull.Value;

				command.Parameters.Add("@i_es_ruc", SqlDbType.Bit);
				command.Parameters["@i_es_ruc"].Value = (object)request.BuscarPorRuc! ?? DBNull.Value;

				command.Parameters.Add("@i_cliente", SqlDbType.VarChar);
				command.Parameters["@i_cliente"].Value = (object)request.Cliente! ?? DBNull.Value;

				command.Parameters.Add("@i_estado", SqlDbType.VarChar);
				command.Parameters["@i_estado"].Value = (object)request.Estado! ?? DBNull.Value;

				command.Parameters.Add("@i_pedido", SqlDbType.Int);
				command.Parameters["@i_pedido"].Value = (object)request.IdPedido! ?? DBNull.Value;

				command.Parameters.Add("@i_orden", SqlDbType.Int);
				command.Parameters["@i_orden"].Value = (object)request.IdOrden! ?? DBNull.Value;

				command.Parameters.Add("@i_idAsesor", SqlDbType.Int);
				command.Parameters["@i_idAsesor"].Value = (object)request.IdAsesor! ?? DBNull.Value;

				await connection.OpenAsync();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						datos.ResumenMuestra = ConvertTo<OrdenLaboratoristaCab>(dataSet.Tables[1]);
						datos.Ordenes = ConvertToList<OrdenLaboratoristaDet>(dataSet.Tables[0]);
					}
					return datos;
				}
			}
		}				
	}

	public async Task<VerOrdenLaboratoristaResponse> VerOrdenLaboratorista(BuscarOrdenLaboratoristaRequest request)
	{
		VerOrdenLaboratoristaResponse datos = new VerOrdenLaboratoristaResponse();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureOrdenesLaboratorista, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "CD";

				command.Parameters.Add("@i_fecha_desde", SqlDbType.DateTime);
				command.Parameters["@i_fecha_desde"].Value = DBNull.Value;

				command.Parameters.Add("@i_fecha_hasta", SqlDbType.DateTime);
				command.Parameters["@i_fecha_hasta"].Value = DBNull.Value;

				command.Parameters.Add("@i_operador", SqlDbType.VarChar);
				command.Parameters["@i_operador"].Value = DBNull.Value;

				command.Parameters.Add("@i_es_ruc", SqlDbType.Bit);
				command.Parameters["@i_es_ruc"].Value = DBNull.Value;

				command.Parameters.Add("@i_cliente", SqlDbType.VarChar);
				command.Parameters["@i_cliente"].Value = DBNull.Value;

				command.Parameters.Add("@i_estado", SqlDbType.VarChar);
				command.Parameters["@i_estado"].Value = DBNull.Value;

				command.Parameters.Add("@i_pedido", SqlDbType.Int);
				command.Parameters["@i_pedido"].Value = DBNull.Value;

				command.Parameters.Add("@i_orden", SqlDbType.Int);
				command.Parameters["@i_orden"].Value = (object)request.IdOrden! ?? DBNull.Value;

				await connection.OpenAsync();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						datos.Orden = ConvertTo<OrdenCab>(dataSet.Tables[0]);
						datos.PruebasMuestras = ConvertToList<OrdenDet>(dataSet.Tables[1]);
					}
					return datos;
				}
			}
		}					
	}

	private List<EstadosCatalogoResponse> EstadosCatalogo(string nombreMaestro)
	{		
		List<EstadosCatalogoResponse> estados = new List<EstadosCatalogoResponse>();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureEstadosCatalogo, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_maestro", SqlDbType.VarChar);
				command.Parameters["@i_maestro"].Value = nombreMaestro;

				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count == 0)
						return null!;
					if (dataSet.Tables[0].Rows.Count == 0)
						return null!;

					estados = ConvertToList<EstadosCatalogoResponse>(dataSet.Tables[0]);
					return estados;
				}
			}
		}
	}

	public async Task<string> ConsultarOrdenGalileo(int idOrden)
	{
		List<OrdenGalileoResponse> data = new List<OrdenGalileoResponse>();

		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(StringHandler.ProcedureConsultaOrdenGalileo, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					//Envio de parametros
					command.Parameters.Add("@i_accion", SqlDbType.Char);
					command.Parameters["@i_accion"].Value = "C";

					command.Parameters.Add("@i_orden", SqlDbType.Int);
					command.Parameters["@i_orden"].Value = idOrden;

					await connection.OpenAsync();

					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataSet dataSet = new DataSet();
						adapter.Fill(dataSet);

						if (dataSet.Tables[0].Rows.Count > 0)
							data = ConvertToList<OrdenGalileoResponse>(dataSet.Tables[0]);

						return ArmaJsonOrdenGalileo(data);
					}
				}
			}				
		}
		catch (Exception e)
		{
			return e.Message;
		}
	}

	public async Task<string> RecibirOrden(int idOrden, string codExterno)
	{
		string resp = string.Empty;

		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(StringHandler.ProcedureOrdenesLaboratorista, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					//Envio de parametros
					command.Parameters.Add("@i_accion", SqlDbType.Char);
					command.Parameters["@i_accion"].Value = "M";

					command.Parameters.Add("@i_fecha_desde", SqlDbType.DateTime);
					command.Parameters["@i_fecha_desde"].Value = DBNull.Value;

					command.Parameters.Add("@i_fecha_hasta", SqlDbType.DateTime);
					command.Parameters["@i_fecha_hasta"].Value = DBNull.Value;

					command.Parameters.Add("@i_operador", SqlDbType.VarChar);
					command.Parameters["@i_operador"].Value = DBNull.Value;

					command.Parameters.Add("@i_es_ruc", SqlDbType.Bit);
					command.Parameters["@i_es_ruc"].Value = DBNull.Value;

					command.Parameters.Add("@i_cliente", SqlDbType.VarChar);
					command.Parameters["@i_cliente"].Value = DBNull.Value;

					command.Parameters.Add("@i_estado", SqlDbType.VarChar);
					command.Parameters["@i_estado"].Value = DBNull.Value;

					command.Parameters.Add("@i_pedido", SqlDbType.Int);
					command.Parameters["@i_pedido"].Value = DBNull.Value;

					command.Parameters.Add("@i_orden", SqlDbType.Int);
					command.Parameters["@i_orden"].Value = idOrden;

					command.Parameters.Add("@i_codExterniLis", SqlDbType.VarChar);
					command.Parameters["@i_codExterniLis"].Value = codExterno.ToString();

					await connection.OpenAsync();

					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataSet dataSet = new DataSet();
						adapter.Fill(dataSet);

						if (dataSet.Tables[0].Rows.Count > 0)
							resp = JsonConvert.SerializeObject(dataSet.Tables[0]);

						return resp;
					}
				}
			}					
		}
		catch (Exception e)
		{
			Dictionary<string, object> errorInfo = new Dictionary<string, object>
				{
					{ "IdOrden", idOrden },
					{ "Mensaje", e.Message }
				};
			return JsonConvert.SerializeObject(errorInfo, Formatting.Indented);
		}
	}

	public async Task<CambiarPruebaResponse> CambiarPrueba(CambiarPruebaRequest request)
	{

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureCambiaPrueba, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "M";

				command.Parameters.Add("@i_id_prueba", SqlDbType.Int);
				command.Parameters["@i_id_prueba"].Value = (object)request.IdPrueba ?? DBNull.Value;

				command.Parameters.Add("@i_id_muestra", SqlDbType.Int);
				command.Parameters["@i_id_muestra"].Value = (object)request.IdMuestra ?? DBNull.Value;

				command.Parameters.Add("@i_rechaza", SqlDbType.Bit);
				command.Parameters["@i_rechaza"].Value = (object)request.Rechaza ?? DBNull.Value;

				command.Parameters.Add("@i_usuario", SqlDbType.Int);
				command.Parameters["@i_usuario"].Value = (object)request.Usuario ?? DBNull.Value;

				command.Parameters.Add("@i_observacion", SqlDbType.VarChar);
				command.Parameters["@i_observacion"].Value = (object)request.Observacion ?? DBNull.Value;

				await connection.OpenAsync();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						CambiarPruebaResponse respuesta = new CambiarPruebaResponse();
						respuesta.IdMuestra = Convert.ToInt32(dataSet.Tables[0].Rows[0][0]);
						respuesta.IdPrueba = Convert.ToInt32(dataSet.Tables[0].Rows[0][1]);
						respuesta.RechazaMuestra = Convert.ToBoolean(dataSet.Tables[0].Rows[0][2]);
						respuesta.ActivaMuestra = Convert.ToBoolean(dataSet.Tables[0].Rows[0][3]);
						respuesta.EstadoPrueba = dataSet.Tables[0].Rows[0][4].ToString()!;

						return respuesta;
					}
					return new CambiarPruebaResponse(request.IdPrueba.ToString(), "Error al actualizar la prueba");
				}
			}
		}									
	}

	public async Task<List<PruebasResponse>> ConsultaPruebas(int idMuestra)
	{
		try
		{
			List<PruebasResponse> pruebas = new List<PruebasResponse>();

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(StringHandler.ProcedureCambiaPrueba, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					//Envio de parametros
					command.Parameters.Add("@i_accion", SqlDbType.Char);
					command.Parameters["@i_accion"].Value = "C";

					command.Parameters.Add("@i_id_prueba", SqlDbType.Int);
					command.Parameters["@i_id_prueba"].Value = DBNull.Value;

					command.Parameters.Add("@i_id_muestra", SqlDbType.Int);
					command.Parameters["@i_id_muestra"].Value = (object)idMuestra ?? DBNull.Value;

					command.Parameters.Add("@i_rechaza", SqlDbType.Bit);
					command.Parameters["@i_rechaza"].Value = DBNull.Value;

					command.Parameters.Add("@i_usuario", SqlDbType.Int);
					command.Parameters["@i_usuario"].Value = DBNull.Value;

					command.Parameters.Add("@i_observacion", SqlDbType.VarChar);
					command.Parameters["@i_observacion"].Value = DBNull.Value;

					await connection.OpenAsync();

					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataSet dataSet = new DataSet();
						adapter.Fill(dataSet);

						if (dataSet.Tables[0].Rows.Count > 0)
						{
							pruebas = ConvertToList<PruebasResponse>(dataSet.Tables[0]);
						}
						return pruebas;
					}
				}
			}													
		}
		catch (Exception e)
		{
			List<PruebasResponse> error = new List<PruebasResponse>();
			error.Add(new PruebasResponse(idMuestra.ToString(), e.Message));
			return error;
		}
	}

	public async Task<string> ActualizaPedidoVerif(int idPedido)
	{
		string Actualizado = "";
		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(StringHandler.ProcedureVerificarActPedi, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					//Envio de parametros
					command.Parameters.Add("@i_accion", SqlDbType.Char);
					command.Parameters["@i_accion"].Value = "C";

					command.Parameters.Add("@i_idPedido", SqlDbType.Int);
					command.Parameters["@i_idPedido"].Value = idPedido;

					await connection.OpenAsync();

					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataSet dataSet = new DataSet();
						adapter.Fill(dataSet);

						if (dataSet.Tables[0].Rows.Count > 0)
						{
							Actualizado = dataSet.Tables[0].Rows[0][0].ToString()!;
						}
					}
				}
			}			
		}
		catch (Exception e)
		{
			return e.Message;
		}
		return Actualizado;
	}

	#endregion

	#region conversiones
	public static T ConvertTo<T>(DataTable dataTable) where T : new()
	{
		T obj = new T();
		foreach (DataRow row in dataTable.Rows)
		{
			foreach (DataColumn column in dataTable.Columns)
			{
				PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName)!;
				if (prop != null && row[column] != DBNull.Value)
				{
					prop.SetValue(obj, Convert.ChangeType(row[column], prop.PropertyType), null);
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

	#endregion

	#region armar json

	private string ArmaJsonOrdenGalileo(List<OrdenGalileoResponse> orden)
	{

		string nomMedico = "";
		string apeMedico = "";

		foreach (var listOrden in orden)
		{
			string[] nomApe = listOrden.Nombre.Split(' ');
			int indiceParentesis = Array.IndexOf(nomApe, "(");

			if (indiceParentesis != -1 || indiceParentesis > 1)
			{
				apeMedico = string.Join(" ", nomApe, 0, indiceParentesis);
				nomMedico = string.Join(" ", nomApe, indiceParentesis + 1, nomApe.Length - indiceParentesis - 2);
			}
			else
			{
                //apeMedico = string.Join(" ", nomApe, 0, Math.Max(1, nomApe.Length - 1));
                //apeMedico = string.Join(" ", nomApe, 0, 2);
                apeMedico = string.Join(" ", nomApe.Take(2));
                //nomMedico = nomApe[nomApe.Length - 1];
                if (nomApe.Length > 2)
				{
                    nomMedico = string.Join(" ", nomApe, 2, nomApe.Length - 3);
                }
				else
				{
                    nomMedico = string.Empty;
                }
			}
		}

		var grupos = orden.GroupBy(p => new
		{
			p.Cliente,
			p.FechaIngreso,
			p.Identificador,
			p.NombrePaciente,
			p.ApellidoPaciente,
			p.FechaNacimiento,
			p.Genero,
			p.CorreoElectronico,
			p.Nombre,
			p.Apellido,
			p.Matricula,
			p.Telefono,	
			p.RucCliente,
			p.Email,
			p.codigoExterno
		})
		.Select(g => new InfoOrden
		{
			idLaboratorio = StringHandler.IdLaboratorio,
			idSede = StringHandler.IdSede,
            //Laboratorio = StringHandler.Laboratorio,
            Laboratorio = g.Key.RucCliente,
            Cliente = g.Key.Cliente,
			CieDiagnostico = StringHandler.CieDiagnostico,
			IdSistemaExterno = StringHandler.IdSistemaExterno,
			//RucCliente = g.Key.RucCliente,
            Paciente = new Paciente
			{
				Identificador = g.Key.Identificador,
				NombrePaciente = g.Key.NombrePaciente,
				ApellidoPaciente = g.Key.ApellidoPaciente,
				FechaNacimiento = g.Key.FechaNacimiento,
				Genero = g.Key.Genero,
				CorreoElectronico = g.Key.CorreoElectronico,
				CamposAdicional = new List<PacienteCampoAdicional>
				{
						new PacienteCampoAdicional
						{
							nombreCampo = g.FirstOrDefault()!.nombreCampoAdicional,
							valor = g.FirstOrDefault()!.valor
						}
				}
			},
			Medico = new Medico
			{
				//Nombre = g.Key.Nombre,
				Nombre = "",
				//Apellido = g.Key.Apellido,
				Apellido = "",
				Matricula = "",
				Telefono = "",
				Email = ""
			},
			prioridadOrden = StringHandler.PrioridadOrden,
			fechaIngreso = g.Key.FechaIngreso,
			codigoExterno = g.Key.codigoExterno,
			usuario = StringHandler.Usuario,
			comentario = g.Select(p => p.Comentario).FirstOrDefault(),
			detalleOrden = g.Select(p => new DetalleOrden
			{
				NombreExamenPerfil = p.NombreExamenPerfil,
				CodigoExamen = p.CodigoExamen
			}).ToList(),
			informacionOrden = new List<InformacionOrden> {
					new InformacionOrden
					{
						nombreCampo = StringHandler.CampoMedicamentos,
						valor = g.FirstOrDefault().medicamento
					},new InformacionOrden
					{
						nombreCampo = StringHandler.CampoDiagnostico,
						valor = g.FirstOrDefault().diagnostico
					}
			},
			tomaMuestra = new TomaMuestra
			{
				tomaRemota = StringHandler.TomaRemota,
				direccionToma = StringHandler.DireccionToma,
				activo = StringHandler.TomaMuestraActivo,
			},
			activo = StringHandler.InfoOrdenActivo

		})
		.SingleOrDefault();

		string json = JsonConvert.SerializeObject(grupos);

		return json;
	}

	#endregion
}
