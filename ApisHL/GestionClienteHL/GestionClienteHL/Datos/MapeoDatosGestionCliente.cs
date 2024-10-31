using GestionClienteHL.Entidades.Consultas;
using GestionClienteHL.Entidades.Operaciones;
using GestionClienteHL.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using System.Reflection.Metadata;

namespace GestionClienteHL.Datos;

public interface IMapeoDatosGestionCliente
{
	List<EstadosHumalabClienteResponse> ConsultarEstadosHumaCliente();
	List<ClientesHumalabEstados> ConsultarClienteHuma(ConsultarClienteHumaQuery queryClienteHuma);
	List<ClientesHumalabEstados> ConsultarClientesHumalab(ConsultarClientesQuery queryCliente);
	GestionClienteEstadoResponse GestionClienteEstadoHuma(GestionClienteEstadoRequest request);
	DarAltaGrabarClienteHumalabResponse GrabarClienteHumalab(DarAltaGrabarClienteHumalabRequest request);
	DarAltaGrabarClienteHumalabResponse ModificarHumalabDarAlta(DarAltaGrabarClienteHumalabRequest request);
	List<PedidoHumalabResponse> PedidosHumalab(int idCliente);
	PedidoOrdenHumalabResponse PedidoOrdenHumalab(string numRemision, int idCliente);
	GraficarPedidosHumalabResponse PedidosEstadosHumalab(GraficarPedidosHumalabRequest graficarPedidos);
	GraficarPedidosHumalabResponse PedidosTodosHumalab(int idCliente);
	MuestrasHumalabResponse MuestrasHumalab(int idCliente);
	string ConsultarObservacionCliente(int idUsuario);
	ClienteHumalabDirecc ConsultarDirecCliente(string ruc, string accion);
}
public class MapeoDatosGestionCliente: IMapeoDatosGestionCliente
{
	private readonly string connectionString;

	#region constructor

	public MapeoDatosGestionCliente()
	{
		connectionString = Environment.GetEnvironmentVariable(StringHandler.DataBaseDev)!;

		if (string.IsNullOrEmpty(connectionString))
		{
			throw new InvalidOperationException("La cadena de conexión no está configurada en las variables de entorno.");
		}

		// Configura el SqlConnectionStringBuilder con la cadena de conexión obtenida
		SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
		connectionString = builder.ConnectionString;
	}

	#endregion

	#region interfaces comunicacion

	List<EstadosHumalabClienteResponse> IMapeoDatosGestionCliente.ConsultarEstadosHumaCliente()
	{
		return EstadosHumaCliente();
	}

	List<ClientesHumalabEstados> IMapeoDatosGestionCliente.ConsultarClienteHuma(ConsultarClienteHumaQuery queryClienteHuma)
	{
		return ObtenerClienteHumalab(queryClienteHuma);
	}

	List<ClientesHumalabEstados> IMapeoDatosGestionCliente.ConsultarClientesHumalab(ConsultarClientesQuery queryCliente)
	{
		return ObtenerClientesHumalab(queryCliente);
	}

	GestionClienteEstadoResponse IMapeoDatosGestionCliente.GestionClienteEstadoHuma(GestionClienteEstadoRequest request)
	{
		return GestionEstadoClienteHuma(request);
	}

	DarAltaGrabarClienteHumalabResponse IMapeoDatosGestionCliente.GrabarClienteHumalab(DarAltaGrabarClienteHumalabRequest request)
	{
		return GrabarClienteHuma(request);
	}

	DarAltaGrabarClienteHumalabResponse IMapeoDatosGestionCliente.ModificarHumalabDarAlta(DarAltaGrabarClienteHumalabRequest request)
	{
		return ModificarHumalabCliente(request);
	}

	List<PedidoHumalabResponse> IMapeoDatosGestionCliente.PedidosHumalab(int idCliente)
	{
		return PedidosHumalab(idCliente);
	}

	PedidoOrdenHumalabResponse IMapeoDatosGestionCliente.PedidoOrdenHumalab(string numRemision, int idCliente)
	{
		return PedidoOrdenHumalab(numRemision, idCliente);
	}

	GraficarPedidosHumalabResponse IMapeoDatosGestionCliente.PedidosEstadosHumalab(GraficarPedidosHumalabRequest graficarPedidos)
	{
		return PedidosEstadosHumalab(graficarPedidos);
	}

	GraficarPedidosHumalabResponse IMapeoDatosGestionCliente.PedidosTodosHumalab(int idCliente)
	{
		return PedidosTodosHumalabGrafico(idCliente);
	}

	MuestrasHumalabResponse IMapeoDatosGestionCliente.MuestrasHumalab(int idCliente)
	{
		return MuestrasHumalab(idCliente);
	}

	string IMapeoDatosGestionCliente.ConsultarObservacionCliente(int idUsuario)
	{
		return ClienteObservacion(idUsuario);
	}

	ClienteHumalabDirecc IMapeoDatosGestionCliente.ConsultarDirecCliente(string ruc, string accion)
	{
		return DireccionCliente(ruc, accion);
	}

	#endregion

	#region conexion BD select, update, insert, delete

	private List<EstadosHumalabClienteResponse> EstadosHumaCliente()
	{		
		List<EstadosHumalabClienteResponse> estados = new List<EstadosHumalabClienteResponse>();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureEstadosHumaCliente, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
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

					estados = ConvertToList<EstadosHumalabClienteResponse>(dataSet.Tables[0]);
					return estados;
				}
			}
		}
	}

	private List<ClientesHumalabEstados> ObtenerClienteHumalab(ConsultarClienteHumaQuery queryClienteHuma)
	{		
		List<ClientesHumalabEstados> clienteHumalab = new List<ClientesHumalabEstados>();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureClienteHumalab, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@i_tipobus", SqlDbType.VarChar);
				command.Parameters["@i_tipobus"].Value = queryClienteHuma.RucNombre;

				command.Parameters.Add("@i_estado", SqlDbType.Int);
				command.Parameters["@i_estado"].Value = queryClienteHuma.idEstado;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count == 0)
						return null!;
					if (dataSet.Tables[0].Rows.Count == 0)
						return null!;

					var totalRegistros = Convert.ToInt32(dataSet.Tables[0].Rows[0]["total_registros"]);
					clienteHumalab = ConvertToList<ClientesHumalabEstados>(dataSet.Tables[1]);
					return clienteHumalab;
					//return new ClientesHumalabEstados(clienteHumalab, totalRegistros, queryClienteHuma.Limit);
				}
			}
		}	
	}

	private List<ClientesHumalabEstados> ObtenerClientesHumalab(ConsultarClientesQuery queryCliente)
	{		
		List<ClientesHumalabEstados> clienteHumalab = new List<ClientesHumalabEstados>();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureConsultaClientesHumalab, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
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

					//consulta pagina
					var totalRegistros = Convert.ToInt32(dataSet.Tables[0].Rows[0]["total_registros"]);
					clienteHumalab = ConvertToList<ClientesHumalabEstados>(dataSet.Tables[1]);
					return clienteHumalab;
					//return new PagedCollection<ClientesHumalabEstados>(clienteHumalab, totalRegistros, queryCliente.Limit);
				}
			}
		}
	}

	private GestionClienteEstadoResponse GestionEstadoClienteHuma(GestionClienteEstadoRequest request)
	{
		GestionClienteEstadoResponse response = new GestionClienteEstadoResponse();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureGestionEstadoCliente, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "M";

				command.Parameters.Add("@i_idCliente", SqlDbType.Int);
				command.Parameters["@i_idCliente"].Value = request.idCliente;

				command.Parameters.Add("@i_cliente", SqlDbType.VarChar);
				command.Parameters["@i_cliente"].Value = request.Cliente;

				command.Parameters.Add("@i_fechaVigencia", SqlDbType.VarChar);
				command.Parameters["@i_fechaVigencia"].Value = request.FechaVigencia;

				command.Parameters.Add("@i_idEstadoNuevo", SqlDbType.Int);
				command.Parameters["@i_idEstadoNuevo"].Value = request.IdEstado;

				command.Parameters.Add("@i_observacion", SqlDbType.VarChar);
				command.Parameters["@i_observacion"].Value = request.Observacion;

				command.Parameters.Add("@i_usuModifica", SqlDbType.Int);
				command.Parameters["@i_usuModifica"].Value = request.UsuarioModificacion;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					var mensaje = Convert.ToString(dataSet.Tables[0].Rows[0]["Column1"]);
					var codigo = Convert.ToString(dataSet.Tables[0].Rows[0]["Column2"]);

					response.MensajeRespuesta = mensaje!;
					response.CodigoRespuesta = codigo!;

					return response;
				}
			}
		}	
	}

	private DarAltaGrabarClienteHumalabResponse GrabarClienteHuma(DarAltaGrabarClienteHumalabRequest request)
	{
		DarAltaGrabarClienteHumalabResponse response = new DarAltaGrabarClienteHumalabResponse();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureGrabarClienteHumalab, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "I";

				command.Parameters.Add("@i_id_galileo", SqlDbType.Int);
				command.Parameters["@i_id_galileo"].Value = request.idGalileo;

				command.Parameters.Add("@i_usuario", SqlDbType.VarChar);
				command.Parameters["@i_usuario"].Value = request.Usuario;

				command.Parameters.Add("@i_identificacion", SqlDbType.VarChar);
				command.Parameters["@i_identificacion"].Value = request.Identificacion;

				command.Parameters.Add("@i_id_rol", SqlDbType.Int);
				command.Parameters["@i_id_rol"].Value = request.IdRol;

				command.Parameters.Add("@i_email", SqlDbType.VarChar);
				command.Parameters["@i_email"].Value = request.Email;

				command.Parameters.Add("@i_usuario_creacion", SqlDbType.Int);
				command.Parameters["@i_usuario_creacion"].Value = request.UsuarioCreacion;

				command.Parameters.Add("@i_usuario_asesor", SqlDbType.VarChar);
				command.Parameters["@i_usuario_asesor"].Value = request.idAsesorFlex;

				command.Parameters.Add("@i_nombre_asesor", SqlDbType.VarChar);
				command.Parameters["@i_nombre_asesor"].Value = request.nombreAsesor;

				command.Parameters.Add("@i_nombre_galileo", SqlDbType.VarChar);
				command.Parameters["@i_nombre_galileo"].Value = request.nombreGalileo;

				command.Parameters.Add("@i_codCliente", SqlDbType.VarChar);
				command.Parameters["@i_codCliente"].Value = request.codigoCliente;

				command.Parameters.Add("@i_direccion", SqlDbType.VarChar);
				command.Parameters["@i_direccion"].Value = request.DireccionCliente;

				command.Parameters.Add("@i_provincia", SqlDbType.VarChar);
				command.Parameters["@i_provincia"].Value = request.ProvinciaCliente;

				command.Parameters.Add("@i_ciudad", SqlDbType.VarChar);
				command.Parameters["@i_ciudad"].Value = request.CiudadCliente;

				command.Parameters.Add("@i_latitud", SqlDbType.VarChar);
				command.Parameters["@i_latitud"].Value = request.LatitudCliente;

				command.Parameters.Add("@i_longitud", SqlDbType.VarChar);
				command.Parameters["@i_longitud"].Value = request.LongitudCliente;

				command.Parameters.Add("@id_OperadorLis", SqlDbType.VarChar);
				command.Parameters["@id_OperadorLis"].Value = request.IdAsesorLis;

				command.Parameters.Add("@i_telefono", SqlDbType.VarChar);
				command.Parameters["@i_telefono"].Value = request.Telefono;

				command.Parameters.Add("@i_labComercial", SqlDbType.VarChar);
				command.Parameters["@i_labComercial"].Value = request.LabComercial;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					var mensaje = Convert.ToString(dataSet.Tables[0].Rows[0]["Column1"]);
					var codigo = Convert.ToString(dataSet.Tables[0].Rows[0]["Column2"]);

					response.MensajeRespuesta = mensaje!;
					response.CodigoRespuesta = codigo!;

					return response;
				}
			}
		}		
	}

	private DarAltaGrabarClienteHumalabResponse ModificarHumalabCliente(DarAltaGrabarClienteHumalabRequest request)
	{
		DarAltaGrabarClienteHumalabResponse response = new DarAltaGrabarClienteHumalabResponse();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureGrabarClienteHumalab, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "M";

				command.Parameters.Add("@i_id_galileo", SqlDbType.VarChar);
				command.Parameters["@i_id_galileo"].Value = request.idGalileo;

				command.Parameters.Add("@i_usuario", SqlDbType.VarChar);
				command.Parameters["@i_usuario"].Value = request.Usuario;

				command.Parameters.Add("@i_identificacion", SqlDbType.VarChar);
				command.Parameters["@i_identificacion"].Value = request.Identificacion;

				command.Parameters.Add("@i_id_rol", SqlDbType.Int);
				command.Parameters["@i_id_rol"].Value = request.IdRol;

				command.Parameters.Add("@i_email", SqlDbType.VarChar);
				command.Parameters["@i_email"].Value = request.Email;

				command.Parameters.Add("@i_usuario_creacion", SqlDbType.Int);
				command.Parameters["@i_usuario_creacion"].Value = request.UsuarioCreacion;

				command.Parameters.Add("@i_usuario_asesor", SqlDbType.VarChar);
				command.Parameters["@i_usuario_asesor"].Value = request.idAsesorFlex;

				command.Parameters.Add("@i_nombre_asesor", SqlDbType.VarChar);
				command.Parameters["@i_nombre_asesor"].Value = request.nombreAsesor;

				command.Parameters.Add("@i_nombre_galileo", SqlDbType.VarChar);
				command.Parameters["@i_nombre_galileo"].Value = request.nombreGalileo;

				command.Parameters.Add("@i_codCliente", SqlDbType.VarChar);
				command.Parameters["@i_codCliente"].Value = request.codigoCliente;

				command.Parameters.Add("@i_direccion", SqlDbType.VarChar);
				command.Parameters["@i_direccion"].Value = request.DireccionCliente;

				command.Parameters.Add("@i_provincia", SqlDbType.VarChar);
				command.Parameters["@i_provincia"].Value = request.ProvinciaCliente;

				command.Parameters.Add("@i_ciudad", SqlDbType.VarChar);
				command.Parameters["@i_ciudad"].Value = request.CiudadCliente;

				command.Parameters.Add("@i_latitud", SqlDbType.VarChar);
				command.Parameters["@i_latitud"].Value = request.LatitudCliente;

				command.Parameters.Add("@i_longitud", SqlDbType.VarChar);
				command.Parameters["@i_longitud"].Value = request.LongitudCliente;

				command.Parameters.Add("@id_OperadorLis", SqlDbType.VarChar);
				command.Parameters["@id_OperadorLis"].Value = request.IdAsesorLis;

				command.Parameters.Add("@i_telefono", SqlDbType.VarChar);
				command.Parameters["@i_telefono"].Value = request.Telefono;

				command.Parameters.Add("@i_labComercial", SqlDbType.VarChar);
				command.Parameters["@i_labComercial"].Value = request.LabComercial;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					var mensaje = Convert.ToString(dataSet.Tables[0].Rows[0]["Column1"]);
					var codigo = Convert.ToString(dataSet.Tables[0].Rows[0]["Column2"]);

					response.MensajeRespuesta = mensaje!;
					response.CodigoRespuesta = codigo!;

					return response;
				}
			}
		}					
	}

	private List<PedidoHumalabResponse> PedidosHumalab(int idCliente)
	{
		List<PedidoHumalabResponse> pedidoHumalab = new();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedurePedidosHumalab, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@i_tipo_consulta", SqlDbType.VarChar);
				command.Parameters["@i_tipo_consulta"].Value = "P";

				command.Parameters.Add("@i_idGalileo", SqlDbType.Int);
				command.Parameters["@i_idGalileo"].Value = idCliente;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count == 0)
						return null!;
					if (dataSet.Tables[0].Rows.Count == 0)
						return null!;

					pedidoHumalab = ConvertToList<PedidoHumalabResponse>(dataSet.Tables[0]);
					return pedidoHumalab;
				}
			}
		}		
	}

	private PedidoOrdenHumalabResponse PedidoOrdenHumalab(string numRemision, int idCliente)
	{
		PedidoOrdenHumalabResponse pedidoOrdenHuma = new();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedurePedidosHumalab, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@i_tipo_consulta", SqlDbType.VarChar);
				command.Parameters["@i_tipo_consulta"].Value = "PO";

				command.Parameters.Add("@i_numRemision", SqlDbType.VarChar);
				command.Parameters["@i_numRemision"].Value = numRemision;

				command.Parameters.Add("@i_idGalileo", SqlDbType.Int);
				command.Parameters["@i_idGalileo"].Value = idCliente;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count == 0)
						return null!;
					if (dataSet.Tables[0].Rows.Count == 0)
						return null!;

					pedidoOrdenHuma.totalOrdenes = Convert.ToInt32(dataSet.Tables[0].Rows[0]["totalOrdenes"]);
					pedidoOrdenHuma.Diagnostico = ConvertToList<DiagnosticoPedido>(dataSet.Tables[1]);

					return pedidoOrdenHuma;
				}
			}
		}				
	}

	private GraficarPedidosHumalabResponse PedidosEstadosHumalab(GraficarPedidosHumalabRequest graficarPedidos)
	{
		GraficarPedidosHumalabResponse graficar = new();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedurePedidosHumalab, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@i_numRemision", SqlDbType.VarChar);
				command.Parameters["@i_numRemision"].Value = graficarPedidos.numRemision;

				command.Parameters.Add("@i_tipo_consulta", SqlDbType.Char);
				command.Parameters["@i_tipo_consulta"].Value = "CDP";

				command.Parameters.Add("@i_idGalileo", SqlDbType.Int);
				command.Parameters["@i_idGalileo"].Value = graficarPedidos.idCliente;

				command.Parameters.Add("@i_diagnostico", SqlDbType.VarChar);
				command.Parameters["@i_diagnostico"].Value = graficarPedidos.nomPrueba;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count == 0)
						return null!;
					if (dataSet.Tables[0].Rows.Count == 0)
						return null!;

					graficar.totalPendientes = Convert.ToInt32(dataSet.Tables[0].Rows[0]["totalPendientes"]);
					graficar.totalProcesadas = Convert.ToInt32(dataSet.Tables[1].Rows[0]["totalProcesadas"]);
					graficar.totalEnviadas = Convert.ToInt32(dataSet.Tables[2].Rows[0]["totalEnviadas"]);

					return graficar;
				}
			}
		}	
	}

	private GraficarPedidosHumalabResponse PedidosTodosHumalabGrafico(int idCliente)
	{
		GraficarPedidosHumalabResponse graficar = new();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedurePedidosHumalab, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@i_tipo_consulta", SqlDbType.Char);
				command.Parameters["@i_tipo_consulta"].Value = "CTP";

				command.Parameters.Add("@i_idGalileo", SqlDbType.Int);
				command.Parameters["@i_idGalileo"].Value = idCliente;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count == 0)
						return null!;
					if (dataSet.Tables[0].Rows.Count == 0)
						return null!;

					graficar.totalPendientes = Convert.ToInt32(dataSet.Tables[0].Rows[0]["totalPendientes"]);
					graficar.totalProcesadas = Convert.ToInt32(dataSet.Tables[1].Rows[0]["totalProcesadas"]);
					graficar.totalEnviadas = Convert.ToInt32(dataSet.Tables[2].Rows[0]["totalEnviadas"]);

					return graficar;
				}
			}
		}		
	}

	private MuestrasHumalabResponse MuestrasHumalab(int idCliente)
	{
		MuestrasHumalabResponse muestras = new();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureMuestrasHumalab, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@idUsuario", SqlDbType.Int);
				command.Parameters["@idUsuario"].Value = idCliente;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count == 0)
						return null!;
					if (dataSet.Tables[0].Rows.Count == 0 && dataSet.Tables[1].Rows.Count == 0)
						return null!;

					muestras.mesActual = ConvertToList<MuestrasMesActual>(dataSet.Tables[0]);
					muestras.mesAnterior = ConvertToList<MuestrasMesAnterior>(dataSet.Tables[1]);

					return muestras;
				}
			}
		}				
	}

	private string ClienteObservacion(int idUsuario)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureObservacionCliente, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@idUsuario", SqlDbType.Int);
				command.Parameters["@idUsuario"].Value = idUsuario;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count == 0)
						return null!;
					if (dataSet.Tables[0].Rows.Count == 0 && dataSet.Tables[1].Rows.Count == 0)
						return null!;

					string observacion = Convert.ToString(dataSet.Tables[0].Rows[0]["Observacion"])!;
					return observacion;
				}
			}
		}		
	}

	private ClienteHumalabDirecc DireccionCliente(string ruc, string accion)
	{
		ClienteHumalabDirecc clientDirec = new ClienteHumalabDirecc();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureDireccCliente, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = Convert.ToChar(accion);

				command.Parameters.Add("@i_ruc", SqlDbType.VarChar);
				command.Parameters["@i_ruc"].Value = ruc;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count == 0)
						return null!;

					clientDirec = ConvertTo<ClienteHumalabDirecc>(dataSet.Tables[0]);
					clientDirec.Codigo = Convert.ToString(dataSet.Tables[1].Rows[0]["Codigo"])!;
					return clientDirec;
				}
			}
		}
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
}
