using FacturacionHL.Datos;
using FacturacionHL.Entidades.Consultas;
using Microsoft.AspNetCore.Mvc;

namespace FacturacionHL.Controllers
{
	[ApiController]
	[Route("api/Humalab/Facturacion")]
	public class FacturaController : ControllerBase
	{
			
		private readonly ILogger<FacturaController> _logger;
		private readonly IMapeoDatosFactura mapeoDatosFactura;

		#region constructor
		public FacturaController(
			ILogger<FacturaController> logger,
			IMapeoDatosFactura _mapeoDatosFactura)
		{
			_logger = logger;
			mapeoDatosFactura = _mapeoDatosFactura;
		}

		#endregion

		#region contraladores metodos

		[HttpGet("consultarCliente")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<ClienteResponse>> buscarCliente(
			[FromQuery] ClienteRequest query)
		{
			try
			{
				var ejemplos = new ClienteResponse();
				await Task.Factory.StartNew(() =>
				{
					ejemplos = mapeoDatosFactura.ObtenerInformacionCliente(query);
				});

				return Ok(ejemplos);
			}
			catch (Exception e)
			{
				return null!;
			}
		}

		[HttpGet("consultarClientes")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<ClienteResponse>>> buscarClientes(
			[FromQuery] ClienteRequest query)
		{
			try
			{
				var ejemplos = new List<ClienteResponse>();
				await Task.Factory.StartNew(() =>
				{
					ejemplos = mapeoDatosFactura.ObtenerInformacionClientes();
				});

				return Ok(ejemplos);
			}
			catch (Exception e)
			{
				return null!;
			}
		}
 
		[HttpPost("listar")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<ListarFacturasQuery>>> Lista(
			[FromBody] ClienteRequest query)
		{
			try
			{
				// Ejecución de la operación de datos
				var ejemplos = new List<ListarFacturasQuery>();
				await Task.Factory.StartNew(() =>
				{
					ejemplos = mapeoDatosFactura.ObtenerListadoFacturas(query);
				});

				return Ok(ejemplos);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpPost("actualizarFacturadas")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<string>> actualizarFacturadas(
			[FromBody] ConsolidarFacturacion query)
		{
			try
			{
				List<string> duplicadosOrden = query.datosFactura!.idOrden!.Distinct().ToList();

				var ejemplos = string.Empty;
				foreach (var item in duplicadosOrden)
				{
					query.datosFactura.idOrdenFinal = item;

					// Ejecución de la operación de datos                    
					await Task.Factory.StartNew(() =>
					{
						ejemplos = mapeoDatosFactura.ActualizarListadoFacturas(query);
					});
				}

				return Ok(ejemplos);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("estadosFactu")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<ClienteResponse>> EstadosFactura()
		{
			try
			{
				var ejemplos = new EstadosFactura();

				await Task.Factory.StartNew(() =>
				{
					ejemplos = mapeoDatosFactura.ObtenerEstadosFactu();
				});

				return Ok(ejemplos);
			}
			catch (Exception e)
			{
				return null;
			}
		}
		#endregion

	}
}