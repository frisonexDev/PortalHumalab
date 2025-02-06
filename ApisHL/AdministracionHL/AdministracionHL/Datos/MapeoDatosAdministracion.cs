using AdministracionHL.Entidades.Consultas;
using AdministracionHL.Utils;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Transactions;
using static AdministracionHL.Utils.Constantes;

namespace AdministracionHL.Datos;

public interface IMapeoDatosAdministracion
{
    bool ConsultarRol(string ruc);
    ClienteHumalab EstadoCliente(string ruc);
    int ActualizaEstadoCliente(string ruc, string estado);
    ClienteResponse ValidaClienteHumalab(string cliente);
    List<CatalogoDetalle> ListarEstadosAdmin(string NombreEstado);
    List<ListarOrden> ListarOrdenes(ConsultarOrden query);
    int EliminarOrdenAdmin(GrabarOrdenRequest request);
    GrabarOrdenRequest ObtenerOrdenAdmin(int IdOrden);
    List<Pruebas> ListarPruebasAdmin(int IdOrden);
    string nombreEstadoOrdAdmin(string idOrdenEstado);
    List<CatalogoTiposClientes> ListarTiposClientesAdmin();
    int ActualizarOrdAdmin(GrabarOrdenRequest request);
    List<Muestras> ListaMuestra(int IdOrden);
    ClienteEtiquetasAdmin ObtenerNombre(int id, string codBarra, int IdMuestra);
    int EliminarPruebasAdmin(Pruebas request);
    string ConsultarPdfFinal(string codBarra);
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

    ClienteEtiquetasAdmin IMapeoDatosAdministracion.ObtenerNombre(int id, string codBarra, int IdMuestra)
    {
        return ObtenerNombreCliente(id, codBarra, IdMuestra);
    }

    GrabarOrdenRequest IMapeoDatosAdministracion.ObtenerOrdenAdmin(int IdOrden)
    {
        GrabarOrdenRequest grabarOrdenRequest = new GrabarOrdenRequest();

        grabarOrdenRequest = ConsultarOrdenAdmin(IdOrden);
        grabarOrdenRequest.Paciente = ObtenerPacienteAdmin(grabarOrdenRequest.Identificacion!);
        return grabarOrdenRequest;
    }

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

    List<CatalogoDetalle> IMapeoDatosAdministracion.ListarEstadosAdmin(string NombreEstado)
    {
        return ListarAdminEstados(NombreEstado);
    }

    List<ListarOrden> IMapeoDatosAdministracion.ListarOrdenes(ConsultarOrden Valor)
    {
        return ListarOrdenes(Valor);
    }

    List<Pruebas> IMapeoDatosAdministracion.ListarPruebasAdmin(int IdOrden)
    {
        List<Pruebas> prueba = new List<Pruebas>();
        prueba = ConsultarPruebasAdmin(IdOrden);
        return prueba;
    }

    string IMapeoDatosAdministracion.nombreEstadoOrdAdmin(string idOrdenEstado)
    {
        return NombreEstadoOrdenAdmin(idOrdenEstado);
    }

    List<CatalogoTiposClientes> IMapeoDatosAdministracion.ListarTiposClientesAdmin()
    {
        return ListarClienteTipos();
    }

    List<Muestras> IMapeoDatosAdministracion.ListaMuestra(int IdOrden)
    {
        return ListarMuestrasAdmin(IdOrden);
    }

    string IMapeoDatosAdministracion.ConsultarPdfFinal(string codBarra)
    {
        return ConsultarBaseResultados(codBarra);
    }

    int IMapeoDatosAdministracion.EliminarOrdenAdmin(GrabarOrdenRequest request)
    {
        int result = 0;
        using (TransactionScope scope = new TransactionScope())
        {
            try
            {
                request.Estado = ObtenerIdDetalleAdmin(Estados.Orden, Estados.Cancelada);
                int estadoPrueba = ObtenerIdDetalleAdmin(Estados.Prueba, Estados.Cancelada);
                int estadoMuestra = ObtenerIdDetalleAdmin(Estados.Muestra, Estados.Cancelada);
                EliminarOrdenAdmin(request);

                List<Pruebas> pruebas = ListarPruebasAdmin(request.IdOrden!.Value);

                if(pruebas.Count > 0)
                {
                    foreach(var item in pruebas)
                    {
                        EliminarPruebaAdmin(item.IdPrueba!.Value, estadoPrueba, request.IdUsuarioGalileo!.Value);
                        List<PruebaMuestra> pruebaMuestra = ObtenerPruebaMuestraAdmin(item.IdPrueba.Value);

                        if(pruebaMuestra.Count > 0)
                        {
                            foreach(var item2 in pruebaMuestra)
                            {
                                EliminarPruebaMuestraAdmin(item2.IdMuestraGalileo, item2.IdPruebaGalileo, request.IdUsuarioGalileo.Value);
                                EliminarMuestraAdmin(item2.IdMuestraGalileo, request.IdOrden.Value, estadoMuestra, request.IdUsuarioGalileo.Value);
                            }
                        }
                    }
                }

                scope.Complete();
                scope.Dispose();
                result = Transaccion.Correcta;
            }
            catch(Exception ex)
            {
                scope.Dispose();
                result = Transaccion.Error;
            }
        }

        return result;
    }

    int IMapeoDatosAdministracion.ActualizarOrdAdmin(GrabarOrdenRequest request)
    {
        int result = 0;

        using (TransactionScope scope = new TransactionScope())
        {
            try
            {
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

                foreach(var prueba in request.Pruebas)
                {
                    prueba.IdOrden = request.IdOrden;
                    prueba.EstadoPrueba = ObtenerIdDetalle(Estados.Prueba, Estados.Generada);

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
            catch(Exception ex)
            {
                scope.Dispose();
                result = Transaccion.Error;
            }
        }

        return result;
    }

    int IMapeoDatosAdministracion.EliminarPruebasAdmin(Pruebas request)
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

                if(existe > Numeros.Cero)
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
            catch(Exception ex)
            {
                scope.Dispose();
                result = Transaccion.Error;
            }
        }

        return result;
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

    private List<CatalogoDetalle> ListarAdminEstados(string NombreEstado)
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

                        if (dataSet.Tables.Count > 0)
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
                            CodigoGalileo = string.IsNullOrEmpty(dataRow.Field<string>("CodigoGalileo")) ? "-" : dataRow.Field<string>("CodigoGalileo")!,
                            NombreCliente = dataRow.Field<string>("NombreCliente")!

                        }).ToList();

                        return lista;
                    }
                    else { return lista; }
                }
            }
        }
    }

    private int ObtenerIdDetalleAdmin(string Estado, string Valor)
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
                    else { 
                        return 0; 
                    }
                }
            }
        }
    }

    private void EliminarOrdenAdmin(GrabarOrdenRequest request)
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

    private List<Pruebas> ListarPruebasAdmin(int IdOrden)
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
                    else { 
                        return lista; 
                    }
                }
            }
        }
    }

    private void EliminarPruebaAdmin(int IdPrueba, int Estado, int UsuarioCreacion)
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

    private List<PruebaMuestra> ObtenerPruebaMuestraAdmin(int IdPrueba)
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
                    else { 
                        return lista; 
                    }
                }
            }
        }
    }

    private void EliminarPruebaMuestraAdmin(int IdMuestra, int IdPrueba, int UsuarioCreacion)
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

    private void EliminarMuestraAdmin(int IdMuestra, int IdOrden, int Estado, int UsuarioCreacion)
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

    private GrabarOrdenRequest ConsultarOrdenAdmin(int IdOrden)
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

    private Pacientes ObtenerPacienteAdmin(string Identificacion)
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

    private List<Pruebas> ConsultarPruebasAdmin(int IdOrden)
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

                    if (dataSet.Tables.Count > 0)
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

    private string NombreEstadoOrdenAdmin(string idOrdenEstado)
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
        catch(Exception ex)
        {
            return lista;
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
                    else { 
                        return 0; 
                    }
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

    private List<Muestras> ListarMuestrasAdmin(int IdOrden)
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

                    if (dataSet.Tables.Count > 0)
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

    private ClienteEtiquetasAdmin ObtenerNombreCliente(int id, string codBarra, int IdMuestra)
    {
        ClienteEtiquetasAdmin clienteEtiquetas = new();

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
                //if (prop != null && row[column] != DBNull.Value)
                //{
                //    prop.SetValue(obj, Convert.ChangeType(row[column], prop.PropertyType), null);
                //}
                if (prop != null && row[column] != DBNull.Value)
                {
                    var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    var safeValue = Convert.ChangeType(row[column], targetType);
                    prop.SetValue(obj, safeValue, null);
                }
            }
        }
        return obj;
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
}
