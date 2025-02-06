using AdministracionHL.Datos;
using AdministracionHL.Entidades.Consultas;
using AdministracionHL.Utils;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using static AdministracionHL.Utils.Constantes;

namespace AdministracionHL.Controllers
{
    [ApiController]
    [Route("api/seguridad/EstadoCliente")]
    public class AdministracionController : ControllerBase
    {        
        private readonly ILogger<AdministracionController> _logger;
        private readonly IMapeoDatosAdministracion mapeoDatosAdministracion;

        public AdministracionController(
            ILogger<AdministracionController> logger,
            IMapeoDatosAdministracion _mapeoDatosAdministracion)
        {
            _logger = logger;
            mapeoDatosAdministracion = _mapeoDatosAdministracion;
        }

        [HttpGet("consultarEstadoHumalab")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClienteHumalab>> estadoClienetHumalab(
            [FromQuery] ClienteHumalab query)
        {
            try
            {                
                //Ejecución de la operación de datos
                var estadoRol = new ClienteHumalab();

                await Task.Factory.StartNew(() =>
                {
                    estadoRol = mapeoDatosAdministracion.EstadoCliente(query.ruc);
                });

                return estadoRol;
            }
            catch (Exception e)
            {
                return null!;
            }
        }

        [HttpGet("actualizaEstado")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> actualizaCliente(
             [FromQuery] string ruc, [FromQuery] string estado)
        {
            try
            {                
                //Ejecución de la operación de datos
                var estadoRol = 0;

                await Task.Factory.StartNew(() =>
                {
                    estadoRol = mapeoDatosAdministracion.ActualizaEstadoCliente(ruc, estado);
                });

                return estadoRol;
            }
            catch (Exception e)
            {
                return null!;
            }
        }

        [HttpGet("existeCliente")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> rolCliente(
            [FromQuery] string ruc)
        {
            try
            {
                var estadoRol = false;

                await Task.Factory.StartNew(() =>
                {
                    estadoRol = mapeoDatosAdministracion.ConsultarRol(ruc);
                });

                return estadoRol;
            }
            catch (Exception e)
            {
                return null!;
            }
        }

        [HttpGet("consultarClienteHumalab")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClienteResponse>> ValidaClienteHumalab(
            [FromQuery] string Ruc)
        {
            try
            {                
                //Ejecución de la operación de datos
                var estadoRol = new ClienteResponse();

                await Task.Factory.StartNew(() =>
                {
                    estadoRol = mapeoDatosAdministracion.ValidaClienteHumalab(Ruc);
                });

                return estadoRol;
            }
            catch (Exception e)
            {
                return null!;
            }
        }

        //Lista de estados para las ordenes
        [HttpGet("listarestadosAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CatalogoDetalle>>> ListarEstadosAdmin(
            [FromQuery] string NombreEstado)
        {
            try
            {
                List<CatalogoDetalle> lista = new List<CatalogoDetalle>();
                await Task.Factory.StartNew(() =>
                {
                    lista = mapeoDatosAdministracion.ListarEstadosAdmin(NombreEstado);
                });

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return null!;
            }
        }

        //Listar ordenes de todos los clientes
        [HttpGet("listarordenesAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ListarOrden>>> ListarOrdenes(
            [FromQuery] ConsultarOrden query)
        {
            List<ListarOrden> lista = new List<ListarOrden>();

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    lista = mapeoDatosAdministracion.ListarOrdenes(query);
                });

                return Ok(lista);
            }
            catch (Exception e)
            {
                return null!;
            }
        }

        //Eliminar orden
        [HttpPost("eliminarordenAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> ElimnarOrdAdmin(
            [FromBody] GrabarOrdenRequest request)
        {
            int response = Transaccion.Default;

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    response = mapeoDatosAdministracion.EliminarOrdenAdmin(request);
                });

                return Created(string.Empty, response);
            }
            catch (Exception ex)
            {
                return null!;
            }
        }

        //Consulta una orden en específico
        [HttpGet("consultarordenAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GrabarOrdenRequest>> ConsultarOrdenAdmin(
            [FromQuery] int IdOrden)
        {
            GrabarOrdenRequest orden = new GrabarOrdenRequest();

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    orden = mapeoDatosAdministracion.ObtenerOrdenAdmin(IdOrden);
                });

                return Ok(orden);
            }
            catch (Exception ex)
            {
                return null!;
            }
        }

        //Consulta pruebas de una orden
        [HttpGet("consultarpruebaAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Pruebas>>> ConsultarPruebasOrdAdmin(
            [FromQuery] int IdOrden, [FromQuery] int IdUsuario)
        {
            List<Pruebas> prueba = new List<Pruebas>();

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    prueba = mapeoDatosAdministracion.ListarPruebasAdmin(IdOrden);
                });

                return Ok(prueba);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //Consulta estado de la orden
        [HttpGet("nombreEstadoOrdenAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> nombreEstadoOrdenAdmin(
            [FromQuery] string idOrdenEstado)
        {
            string nombre = string.Empty;

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    nombre = mapeoDatosAdministracion.nombreEstadoOrdAdmin(idOrdenEstado);
                });

                return Ok(nombre);
            }
            catch (Exception ex)
            {
                return null!;
            }
        }

        [HttpGet("listarTiposClientesAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CatalogoTiposClientes>>> ListarTiposClientesAdm()
        {
            try
            {
                List<CatalogoTiposClientes> lista = new List<CatalogoTiposClientes>();
                await Task.Factory.StartNew(() =>
                {
                    lista = mapeoDatosAdministracion.ListarTiposClientesAdmin();
                });

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return null!;
            }
        }

        [HttpPost("actualizarordenAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DatosResponse>> ActualizarOrdenAdmin(
            [FromBody] GrabarOrdenRequest request)
        {
            int response = Transaccion.Default;

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    response = mapeoDatosAdministracion.ActualizarOrdAdmin(request);
                });

                return Created(string.Empty, response);
            }
            catch (Exception ex)
            {
                return null!;
            }
        }

        [HttpGet("listarmuestrasAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Muestras>>> ListarMuestrasAdmin(
            [FromQuery] int IdOrden)
        {
            List<Muestras> lista = new List<Muestras>();

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    lista = mapeoDatosAdministracion.ListaMuestra(IdOrden);
                });

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return null!;
            }
        }

        [HttpPost("pdfetiquetasAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CodigoBarrasPdf>>> pdfetiquetas(
            [FromBody] List<CodigoBarrasResquest> grabarPeticion)
        {
            List<CodigoBarrasPdf> BarrasCodigo = new List<CodigoBarrasPdf>();

            try
            {                                
                string cliente = string.Empty;
                ClienteEtiquetasAdmin clienteEtiquetas = new ClienteEtiquetasAdmin();

                string codigoFinal = string.Empty;

                foreach (var muestras in grabarPeticion)
                {
                    await Task.Factory.StartNew(() =>
                    {
                        clienteEtiquetas = mapeoDatosAdministracion.ObtenerNombre(muestras.UsuarioCreacion!.Value, muestras.CodigoBarra!, muestras.IdMuestra!.Value);
                    });

                    BarrasCodigo.Add(new CodigoBarrasPdf
                    {
                        Nombre = clienteEtiquetas.cliente,
                        Codigo = muestras.CodigoBarra!,
                        IdentiPaciente = clienteEtiquetas.identiPaciente,
                        NombrePaciente = clienteEtiquetas.nombrePaciente,
                        muestraGalileo = clienteEtiquetas.muestra
                    });
                }

            }
            catch (Exception e)
            {
                return null;
            }

            return BarrasCodigo;
        }

        [HttpPost("eliminarpruebasAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DatosResponse>> EliminarPruebAdmin(
            [FromBody] Pruebas request)
        {
            int response = Transaccion.Default;

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    response = mapeoDatosAdministracion.EliminarPruebasAdmin(request);
                });

                return Created(string.Empty, response);
            }
            catch (Exception ex)
            {
                return null!;
            }
        }

        [HttpGet("resultadoPdfinalAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> ResultadoPdfinalAdmin(
            [FromQuery] string codBarra)
        {
            string pdf = "";

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    pdf = mapeoDatosAdministracion.ConsultarPdfFinal(codBarra);
                });

                return Ok(pdf);
            }
            catch (Exception ex)
            {
                return null!;
            }
        }
    }
}