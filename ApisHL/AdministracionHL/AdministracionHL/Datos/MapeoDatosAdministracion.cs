using AdministracionHL.Entidades.Consultas;
using AdministracionHL.Utils;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Reflection;

namespace AdministracionHL.Datos;

public interface IMapeoDatosAdministracion
{
    bool ConsultarRol(string ruc);
    ClienteHumalab EstadoCliente(string ruc);
    int ActualizaEstadoCliente(string ruc, string estado);
    ClienteResponse ValidaClienteHumalab(string cliente);

}
public class MapeoDatosAdministracion: IMapeoDatosAdministracion
{
    private readonly string connectionString;

    #region constructor

    public MapeoDatosAdministracion()
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

    #region interfaces
    bool IMapeoDatosAdministracion.ConsultarRol(string ruc)
    {
        return ExisteRol(ruc);
    }

    ClienteHumalab IMapeoDatosAdministracion.EstadoCliente(string ruc)
    {
        return ObtenerEstadoCliente(ruc);
    }

    int IMapeoDatosAdministracion.ActualizaEstadoCliente(string ruc, string estado)
    {
        return ActualizaEstadoCliente(ruc, estado);
    }

    ClienteResponse IMapeoDatosAdministracion.ValidaClienteHumalab(string cliente)
    {
        return ValidaClienteHumalab(cliente);
    }

    #endregion

    #region Mapeo Datos

    private ClienteHumalab ObtenerEstadoCliente(string ruc)
    {
        var respuestaEstado = new ClienteHumalab();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(StringHandler.ProcedureEstadoCliente, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                //Envio de parametros
                command.Parameters.Add("@i_accion", SqlDbType.Char);
                command.Parameters["@i_accion"].Value = "C";

                command.Parameters.Add("@i_ruc", SqlDbType.VarChar);
                command.Parameters["@i_ruc"].Value = ruc;

                connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    if (dataSet.Tables.Count == 0)
                        return null!;
                    if (dataSet.Tables[0].Rows.Count == 0)
                        return null!;

                    respuestaEstado = ConvertTo<ClienteHumalab>(dataSet.Tables[0]);
                    return respuestaEstado;
                }
            }
        }
    }

    private int ActualizaEstadoCliente(string ruc, string estado)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(StringHandler.ProcedureEstadoCliente, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                //Envio de parametros
                command.Parameters.Add("@i_accion", SqlDbType.VarChar);
                command.Parameters["@i_accion"].Value = "M";

                command.Parameters.Add("@i_estado", SqlDbType.VarChar);
                command.Parameters["@i_estado"].Value = estado;

                command.Parameters.Add("@i_ruc", SqlDbType.VarChar);
                command.Parameters["@i_ruc"].Value = ruc;

                connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    if (dataSet.Tables.Count == 0)
                        return 0;
                    if (dataSet.Tables[0].Rows.Count == 0)
                        return 0;

                    var respuestaEstado = Convert.ToInt32(dataSet.Tables[0].Rows[0]["actualizados"]);
                    return respuestaEstado;
                }
            }
        }
    }

    private bool ExisteRol(string ruc)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(StringHandler.ProcedureExisteRol, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                
                command.Parameters.Add("@i_ruc", SqlDbType.VarChar);
                command.Parameters["@i_ruc"].Value = ruc;

                connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    if (dataSet.Tables.Count == 0)
                        return false;
                    if (dataSet.Tables[0].Rows.Count == 0)
                        return false;

                    var respuestaEstado = Convert.ToString(dataSet.Tables[0].Rows[0]["estado"]);
                    return respuestaEstado!.Equals("Activo") ? true : false;
                }
            }
        }        
    }

    private ClienteResponse ValidaClienteHumalab(string cliente)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(StringHandler.ProcedureValidaCliente, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@i_accion", SqlDbType.VarChar);
                command.Parameters["@i_accion"].Value = "C";

                command.Parameters.Add("@i_ruc", SqlDbType.VarChar);
                command.Parameters["@i_ruc"].Value = cliente;

                connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    if (dataSet.Tables.Count == 0)
                        return null!;
                    if (dataSet.Tables[0].Rows.Count == 0)
                        return null!;

                    var respuestaEstado = ConvertTo<ClienteResponse>(dataSet.Tables[0]);
                    return respuestaEstado;
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

    #endregion
}
