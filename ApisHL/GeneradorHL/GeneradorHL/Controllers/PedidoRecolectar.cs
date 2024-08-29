using GeneradorHL.Datos;
using GeneradorHL.Documentos.RecolectarPedido;
using GeneradorHL.Entidades.Consultas;
using Microsoft.AspNetCore.Mvc;

namespace GeneradorHL.Controllers;

[Route("api/Humalab/GeneradorHl")]
public class PedidoRecolectar : ControllerBase
{
	private readonly ILogger<PedidoRecolectar> _logger;
	private readonly IMapeoDatosGenerador mapeoDatosGenerador;

	public PedidoRecolectar(
		ILogger<PedidoRecolectar> logger,
		IMapeoDatosGenerador _mapeoDatosEjemplo)
	{
		_logger = logger;
		mapeoDatosGenerador = _mapeoDatosEjemplo!;
	}

	[HttpPost("pdfRecolectarPedido")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<string>> pdfRecolectarPedido(
	[FromHeader] string REFERENCE_ID, [FromHeader] string CONSUMER,
	[FromBody] VerDetallePedidoOperadorResponse pedidoOpe)
	{
		string pedidosBase64 = "";

		try
		{			
			RecolectarPedido pedido = new RecolectarPedido();
			await Task.Factory.StartNew(() =>
			{
				pedidosBase64 = pedido.GenerarDocumento(pedidoOpe);
			});
		}
		catch (Exception e)
		{
			return null!;
		}

		return pedidosBase64;
	}

}
