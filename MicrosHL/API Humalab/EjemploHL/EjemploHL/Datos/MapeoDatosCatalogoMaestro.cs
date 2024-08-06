using ClienteHL.Entidades.Consultas.CatalogoMaestro;
using EjemploHL.Entidades.Consultas.CatalogoMaestro;
using EjemploHL.Utils;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing;

namespace EjemploHL.Datos;

public interface IMapeoDatosCatalogoMaestro
{
	List<CatalogoDetalle> ListarCatalogoDetalle(CatalogoRequest query);
	List<CatalogoDetalle> ListarEstados(string NombreEstado);
	List<CatalogoTiposClientes> ListarTiposClientes();

}

public class MapeoDatosCatalogoMaestro: IMapeoDatosCatalogoMaestro
{	
	private readonly string connectionString;

	List<CatalogoDetalle> IMapeoDatosCatalogoMaestro.ListarCatalogoDetalle(CatalogoRequest query)
	{
		return ListarCatalogoDetalle(query);
	}

	List<CatalogoDetalle> IMapeoDatosCatalogoMaestro.ListarEstados(string NombreEstado)
	{
		return ListarEstado(NombreEstado);
	}

	List<CatalogoTiposClientes> IMapeoDatosCatalogoMaestro.ListarTiposClientes()
	{
		return ListarClienteTipos();
	}

	public MapeoDatosCatalogoMaestro()
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

	private List<CatalogoDetalle> ListarCatalogoDetalle(CatalogoRequest query)
	{
		List<CatalogoDetalle> detalle = new List<CatalogoDetalle>();

		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(StringHandler.ProcedureIdDetalle, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					//Envio de parametros
					command.Parameters.Add("@i_accion", SqlDbType.Char);
					command.Parameters["@i_accion"].Value = "C";

					command.Parameters.Add("@idCatalogoMaestro", SqlDbType.Int);
					command.Parameters["@idCatalogoMaestro"].Value = query.IdCatalogoMaestro;

					connection.Open();

					//lectura de la data
					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataSet dataSet = new DataSet();
						adapter.Fill(dataSet);

						if (dataSet.Tables.Count > 0)
						{
							// Mapear DataSet a una lista de CatalogoDetalle
							detalle = dataSet.Tables[0].AsEnumerable().Select(dataRow => new CatalogoDetalle
							{
								IdCatalogoMaestro = dataRow.Field<int>("IdCatalogoMaestro"),
								IdCatalogoDetalle = dataRow.Field<int>("IdCatalogoDetalle"),
								Relacion = dataRow.Field<int>("Relacion"),
								Nombre = dataRow.Field<string>("Nombre")!,
								Valor = dataRow.Field<string>("Valor")!,
								Editable = dataRow.Field<bool>("Editable"),
								Eliminar = dataRow.Field<bool>("Eliminar")
							}).ToList();

							return detalle;
						}
						else
						{
							return detalle;
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

	private List<CatalogoDetalle> ListarEstado(string NombreEstado)
	{
		List<CatalogoDetalle> lista = new List<CatalogoDetalle>();

		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(StringHandler.ProcedureIdDetalle, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					//Envio de parametros
					command.Parameters.Add("@i_accion", SqlDbType.Char);
					command.Parameters["@i_accion"].Value = "C1";

					command.Parameters.Add("@estado", SqlDbType.VarChar);
					command.Parameters["@estado"].Value = NombreEstado;

					connection.Open();

					//lectura de la data
					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataSet dataSet = new DataSet();
						adapter.Fill(dataSet);

						if(dataSet.Tables.Count > 0)
						{
							// Mapear DataSet a una lista de CatalogoDetalle
							lista = dataSet.Tables[0].AsEnumerable().Select(dataRow => new CatalogoDetalle
							{
								Nombre = dataRow.Field<string>("Estados")!,
								Valor = dataRow.Field<string>("Abreviatura")!
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

	private List<CatalogoTiposClientes> ListarClienteTipos()
	{
		List<CatalogoTiposClientes> lista = new List<CatalogoTiposClientes>();

		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(StringHandler.ProcedureCataTipoClient, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					//Envio de parametros
					command.Parameters.Add("@i_accion", SqlDbType.Char);
					command.Parameters["@i_accion"].Value = "C";

					connection.Open();

					//lectura de la data
					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						DataSet dataSet = new DataSet();
						adapter.Fill(dataSet);

						if (dataSet.Tables.Count > 0)
						{
							// Mapear DataSet a una lista
							lista = dataSet.Tables[0].AsEnumerable().Select(dataRow => new CatalogoTiposClientes
							{
								IdCatalogo = dataRow.Field<int>("IdCatalogo"),
								NombreCatalogo = dataRow.Field<string>("NombreCatalogo")!,
								ValorCatalogo = dataRow.Field<string>("ValorCatalogo")!

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
}


