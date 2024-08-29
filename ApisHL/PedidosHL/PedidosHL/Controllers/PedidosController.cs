using Microsoft.AspNetCore.Mvc;
using PedidosHL.Datos;
using PedidosHL.Entidades.Consultas;
using PedidosHL.Entidades.Operaciones;

namespace PedidosHL.Controllers
{
	[ApiController]
	[Route("api/Humalab/PedidosHl")]
	public class PedidosController : ControllerBase
	{		
		private readonly ILogger<PedidosController> _logger;
		private readonly IMapeoDatosPedido mapeoDatosPedido;

		#region contructor
		public PedidosController(ILogger<PedidosController> logger,
			IMapeoDatosPedido _mapeoDatosPedido)
		{
			_logger = logger;
			mapeoDatosPedido = _mapeoDatosPedido;
		}

		#endregion

		#region consultas

		[HttpGet("buscar")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<BuscarPedidoOperadorResponse>>> ObtenerPedidoOpLogistico(			
			[FromQuery] BuscarPedidoOperadorRequest request)
		{
			try
			{
				var response = await mapeoDatosPedido.BuscarPedidoOperadorAsync(request);
				return Ok(response);
			}
			catch(Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("buscarPedidosActua")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<string>> BuscarPedidosActua(			
			[FromQuery] int idPedido, int idOperador)
		{
			try
			{
				// Ejecución de la operación de datos
				var response = await mapeoDatosPedido.BuscarPedidoOperadorActuAsync(idPedido, idOperador);
				return Ok(response);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("ver")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<BuscarPedidoOperadorResponse>>> VerDetallePedidoOperador(			
			[FromQuery] VerDetallePedidoOperadorRequest request)
		{
			try
			{
				var response = await mapeoDatosPedido.VerDetallePedidoOperadorAsync(request);
				return Ok(response);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("verPedidos")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<BuscarPedidoOperadorResponse>>> VerDetallePedidoOperadorNew(
			[FromQuery] VerDetallePedidoOperadorRequest request)
		{
			try
			{
				var response = await mapeoDatosPedido.VerDetallePedidoOperadorNewAsync(request);
				return Ok(response);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpPut("recogermuestra")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<RecogerMuestraResponse>> RecogerMuestraOperador(
			[FromQuery] RecogerMuestraRequest request)
		{
			try
			{
				var response = await mapeoDatosPedido.RecogerMuestraAsync(request);
				return Ok(response);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpPut("recogerpedido")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<RecogerPedidoResponse>> RecogerPedido(
			[FromQuery] RecogerPedidoRequest request)
		{
			try
			{
				var response = await mapeoDatosPedido.RecogerPedidoAsync(request);
				return Ok(response);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpPut("entregarpedido")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<EntregarPedidoResponse>> EntregarPedido(
			[FromQuery] EntregarPedidoRequest request)
		{
			try
			{
				var response = await mapeoDatosPedido.EntregarPedidoAsync(request);
				return Ok(response);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("consultarordenes")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<OrdenLaboratoristaResponse>>> PedidosLaboratorista(
			[FromQuery] BuscarOrdenLaboratoristaRequest request)
		{
			try
			{
				var response = await mapeoDatosPedido.BuscarOrdenLaboratoristaAsync(request);
				return Ok(response);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("verorden")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<VerOrdenLaboratoristaResponse>>> VerOrdenLaboratorista(
			[FromQuery] BuscarOrdenLaboratoristaRequest request)
		{
			try
			{
				var response = await mapeoDatosPedido.VerOrdenLaboratoristaAsync(request);
				return Ok(response);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("estadosCatalogo")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<EstadosCatalogoResponse>>> ConsultarEstadosCatalogo(
			[FromQuery] string nombreMaestro)
		{
			try
			{
				var estados = new List<EstadosCatalogoResponse>();

				await Task.Factory.StartNew(() =>
				{
					estados = mapeoDatosPedido.ConsultarEstadosCatalogo(nombreMaestro);
				});

				return estados;
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("ordenGalileo")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<OrdenGalileoResponse>>> ConsultarOrdenGalileo(
			[FromQuery] int idOrden)
		{
			try
			{
				var response = await mapeoDatosPedido.ConsultarOrdenGalileoAsync(idOrden);
				return Ok(response);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpPut("recibirOrden")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<object>> RecibirOrden(
			[FromQuery] int idOrden, string codExterno)
		{
			try
			{
				var response = await mapeoDatosPedido.RecibirOrdenAsync(idOrden, codExterno);
				return Ok(response);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpPut("cambiarprueba")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<CambiarPruebaResponse>> CambiarPruebaLaboratorista(
			[FromQuery] CambiarPruebaRequest request)
		{
			try
			{
				var response = await mapeoDatosPedido.CambiarPruebaAsync(request);
				return Ok(response);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("consultapruebas")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<PruebasResponse>>> ConsultaPruebasLaboratorista(
			[FromQuery] int idMuestra)
		{
			try
			{
				var response = await mapeoDatosPedido.ConsultaPruebasAsync(idMuestra);
				return Ok(response);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("pedidoAct")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<string>> pedidoActualizado(
			[FromQuery] int idPedido)
		{
			try
			{
				var response = await mapeoDatosPedido.ActualizaPedido(idPedido);
				return Ok(response);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		#endregion
	}
}