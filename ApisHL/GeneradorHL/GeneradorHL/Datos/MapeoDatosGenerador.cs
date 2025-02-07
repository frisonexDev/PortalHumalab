using GeneradorHL.Entidades.Consultas;
using GeneradorHL.Utils;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GeneradorHL.Datos;
public interface IMapeoDatosGenerador
{
	ClienteEtiquetas ObtenerNombre(int id, string codBarra, int IdMuestra);
}

public class MapeoDatosGenerador: IMapeoDatosGenerador
{

	private readonly string connectionString;

	#region constructor

	public MapeoDatosGenerador()
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

	ClienteEtiquetas IMapeoDatosGenerador.ObtenerNombre(int id, string codBarra, int IdMuestra)
	{
		return ObtenerNombreCliente(id, codBarra, IdMuestra);
	}

	#endregion

	#region Métodos de consulta de la clase
	private ClienteEtiquetas ObtenerNombreCliente(int id, string codBarra, int IdMuestra)
	{		
		ClienteEtiquetas clienteEtiquetas = new();

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			using (SqlCommand command = new SqlCommand(StringHandler.ProcedureNombreCliente, connection))
			{
				command.CommandType = CommandType.StoredProcedure;
				//Envio de parametros
				command.Parameters.Add("@i_accion", SqlDbType.Char);
				command.Parameters["@i_accion"].Value = "C";

				command.Parameters.Add("@i_id", SqlDbType.Int);
				command.Parameters["@i_id"].Value = id;

				command.Parameters.Add("@i_codbarra", SqlDbType.VarChar);
				command.Parameters["@i_codbarra"].Value = codBarra;

				command.Parameters.Add("@id_muestra", SqlDbType.Int);
                command.Parameters["@id_muestra"].Value = IdMuestra;

                connection.Open();

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					if (dataSet.Tables.Count == 0)
						return null!;
					if (dataSet.Tables[0].Rows.Count == 0)
						return null!;

					clienteEtiquetas.cliente = Convert.ToString(dataSet.Tables[0].Rows[0]["NombreCliente"])!;
					clienteEtiquetas.nombrePaciente = Convert.ToString(dataSet.Tables[1].Rows[0]["Nombres"])!;
					clienteEtiquetas.identiPaciente = Convert.ToString(dataSet.Tables[1].Rows[0]["Identificacion"])!;
					clienteEtiquetas.muestra = Convert.ToString(dataSet.Tables[1].Rows[0]["Nombre"])!;

					return clienteEtiquetas;
				}
			}
		}		
	}
	#endregion
}
