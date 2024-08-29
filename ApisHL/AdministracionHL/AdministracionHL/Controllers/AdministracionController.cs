using AdministracionHL.Datos;
using AdministracionHL.Entidades.Consultas;
using Microsoft.AspNetCore.Mvc;

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

    }
}