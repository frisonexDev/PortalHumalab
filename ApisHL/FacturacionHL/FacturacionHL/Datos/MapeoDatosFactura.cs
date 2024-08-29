using FacturacionHL.Entidades.Consultas;
using FacturacionHL.Utils;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace FacturacionHL.Datos;

public interface IMapeoDatosFactura
{
	ClienteResponse ObtenerInformacionCliente(ClienteRequest query);
	List<ClienteResponse> ObtenerInformacionClientes();
	List<ListarFacturasQuery> ObtenerListadoFacturas(ClienteRequest query);
	string ActualizarListadoFacturas(ConsolidarFacturacion query);
	EstadosFactura ObtenerEstadosFactu();
}

public class MapeoDatosFactura: IMapeoDatosFactura
{
	private readonly string connectionString;

	#region constructor

	public MapeoDatosFactura()
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
	ClienteResponse IMapeoDatosFactura.ObtenerInformacionCliente(ClienteRequest query)
	{
		return ObtenerInformacionCliente(query);
	}

	List<ClienteResponse> IMapeoDatosFactura.ObtenerInformacionClientes()
	{
		return ObtenerInformacionClientes();
	}

	List<ListarFacturasQuery> IMapeoDatosFactura.ObtenerListadoFacturas(ClienteRequest query)
	{
		return ObtenerListadoFacturas(query);
	}

	string IMapeoDatosFactura.ActualizarListadoFacturas(ConsolidarFacturacion query)
	{
		return ActuaizarListadoFacturas(query);
	}

	EstadosFactura IMapeoDatosFactura.ObtenerEstadosFactu()
	{
		return FacturaEstados();
	}
	#endregion

	#region datos 

	private ClienteResponse ObtenerInformacionCliente(ClienteRequest query)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureClienteFactura, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@identificador", SqlDbType.VarChar);
				command.Parameters["@identificador"].Value = query.Ruc;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					// Se genera la consulta paginada
					if (dataSet.Tables.Count == 0)
						return null!;
					if (dataSet.Tables[0].Rows.Count == 0)
						return null!;

					var respuestaCliente = ConvertTo<ClienteResponse>(dataSet.Tables[0]);
					return respuestaCliente;
				}
			}
		}
	}

	private List<ClienteResponse> ObtenerInformacionClientes()
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureClienteFactura, connection))
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

					var respuestaCliente = ConvertToList<ClienteResponse>(dataSet.Tables[0]);
					return respuestaCliente;
				}
			}
		}		
	}

	private List<ListarFacturasQuery> ObtenerListadoFacturas(ClienteRequest query)
	{

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureFacturas, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@i_identificador", SqlDbType.VarChar);
				command.Parameters["@i_identificador"].Value = query.Ruc;

				command.Parameters.Add("@i_tipo", SqlDbType.VarChar);
				command.Parameters["@i_tipo"].Value = query.Estado;

				command.Parameters.Add("@i_desde", SqlDbType.VarChar);
				command.Parameters["@i_desde"].Value = query.Desde;

				command.Parameters.Add("@i_hasta", SqlDbType.VarChar);
				command.Parameters["@i_hasta"].Value = query.Hasta;

				command.Parameters.Add("@i_todos", SqlDbType.VarChar);
				command.Parameters["@i_todos"].Value = query.Valor;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count == 0)
						return null!;
					if (dataSet.Tables[0].Rows.Count == 0)
						return null!;

					var respuestaLista = ConvertToList<ListarFacturasQuery>(dataSet.Tables[0]);

					return respuestaLista;
				}
			}
		}		
	}

	private string ActuaizarListadoFacturas(ConsolidarFacturacion query)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureActualizarItemsFactura, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "M";

				command.Parameters.Add("@i_identificador", SqlDbType.VarChar);
				command.Parameters["@i_identificador"].Value = query.clienteRequest!.Ruc;

				command.Parameters.Add("@i_tipo", SqlDbType.VarChar);
				command.Parameters["@i_tipo"].Value = query.clienteRequest.Estado;

				command.Parameters.Add("@i_desde", SqlDbType.VarChar);
				command.Parameters["@i_desde"].Value = query.clienteRequest.Desde;

				command.Parameters.Add("@i_hasta", SqlDbType.VarChar);
				command.Parameters["@i_hasta"].Value = query.clienteRequest.Hasta;

				command.Parameters.Add("@i_numeroFactura", SqlDbType.VarChar);
				command.Parameters["@i_numeroFactura"].Value = query.datosFactura!.numeroFactura;

				command.Parameters.Add("@i_identificacionUsuario", SqlDbType.VarChar);
				command.Parameters["@i_identificacionUsuario"].Value = query.datosFactura.identificacionUsuario;

				command.Parameters.Add("@i_totalFactura", SqlDbType.VarChar);
				command.Parameters["@i_totalFactura"].Value = query.datosFactura.totalFactura;

				command.Parameters.Add("@i_totalMuestras", SqlDbType.VarChar);
				command.Parameters["@i_totalMuestras"].Value = query.datosFactura.totalMuestra;

				command.Parameters.Add("@i_estado", SqlDbType.VarChar);
				command.Parameters["@i_estado"].Value = query.datosFactura.estado;

				command.Parameters.Add("@i_usuarioCreacion", SqlDbType.VarChar);
				command.Parameters["@i_usuarioCreacion"].Value = query.datosFactura.usuarioCreacion;

				command.Parameters.Add("@i_idOrden", SqlDbType.VarChar);
				command.Parameters["@i_idOrden"].Value = query.datosFactura.idOrdenFinal;

				connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count == 0)
						return null!;
					if (dataSet.Tables[0].Rows.Count == 0)
						return null!;

					var respuestaEstado = Convert.ToString(dataSet.Tables[0].Rows[0]["actualizados"]);
					return respuestaEstado!;
				}
			}
		}		
	}

	public EstadosFactura FacturaEstados()
	{
		EstadosFactura estadosFactura = new EstadosFactura();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureEstadosFactu, connection))
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

					// Se genera la consulta paginada
					if (dataSet.Tables.Count == 0)
						return null!;
					if (dataSet.Tables[0].Rows.Count == 0)
						return null!;

					estadosFactura.Validadas = Convert.ToInt32(dataSet.Tables[0].Rows[0]["Validadas"]);
					estadosFactura.Facturdas = Convert.ToInt32(dataSet.Tables[1].Rows[0]["Facturadas"]);

					return estadosFactura;
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
