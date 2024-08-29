using ClienteHL.Entidades.Consultas.Ordenes;
using ClienteHL.Entidades;
using EjemploHL.Utils;
using Microsoft.Data.SqlClient;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace ClienteHL.Datos;

public interface IMapeoDatosPaciente
{
	Paciente ObtenerPaciente(ConsultarPaciente datos);
}

public class MapeoDatosPaciente : IMapeoDatosPaciente
{
	private readonly string connectionString;

	Paciente IMapeoDatosPaciente.ObtenerPaciente(ConsultarPaciente datos)
	{
		return ObtenerPaciente(datos);
	}

	#region constructor
	public MapeoDatosPaciente()
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

	private Paciente ObtenerPaciente(ConsultarPaciente datos)
	{
		var Paciente1 = new Paciente();
		var Paciente = new Paciente();
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
					command.Parameters["@identificacion"].Value = datos.Identificacion;

					connection.Open();

					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataSet dataSet = new DataSet();
						adapter.Fill(dataSet);

						if (dataSet.Tables.Count == 0) return null!;
						if (dataSet.Tables[0].Rows.Count == 0)
						{
							Paciente1 = new Paciente
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
								FechaCreacion = DateTime.Now
							};

							return Paciente1;
						}
						else
						{
							Paciente = ConvertTo<Paciente>(dataSet.Tables[0]);
							return Paciente;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			return null!;
		}				
	}

	#region Convert
	protected T ConvertTo<T>(DataTable table) where T : new()
	{
		List<string> list = new List<string>();
		foreach (DataColumn column in table.Columns)
		{
			list.Add(column.ColumnName);
		}

		return GetObject<T>(table.Rows[0], list);
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
						propertyInfo.SetValue(val, Convert.ChangeType(empty2, Type.GetType(Nullable.GetUnderlyingType(propertyInfo.PropertyType)!.ToString())!), null);
					}
					else if (propertyInfo.PropertyType.IsEnum)
					{
						propertyInfo.SetValue(val, Enum.Parse(propertyInfo.PropertyType, empty2), null);
					}
					else
					{
						propertyInfo.SetValue(val, Convert.ChangeType(empty2, Type.GetType(propertyInfo.PropertyType.ToString())!), null);
					}
				}
				else if (propertyInfo.PropertyType.IsEnum)
				{
					propertyInfo.SetValue(val, Enum.Parse(propertyInfo.PropertyType, empty2), null);
				}
				else
				{
					propertyInfo.SetValue(val, Convert.ChangeType(empty2, Type.GetType(propertyInfo.PropertyType.ToString())!), null);
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
