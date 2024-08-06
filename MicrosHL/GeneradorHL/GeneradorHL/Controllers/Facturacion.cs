using GeneradorHL.Datos;
using GeneradorHL.Documentos.Facturacion;
using GeneradorHL.Entidades.Consultas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeneradorHL.Controllers;

[ApiController]
[Route("api/Humalab/GeneradorHl")]
public class Facturacion : ControllerBase
{
	private readonly ILogger<Facturacion> _logger;
	private readonly IMapeoDatosGenerador mapeoDatosGenerador;

	public Facturacion(
		ILogger<Facturacion> logger,
		IMapeoDatosGenerador _mapeoDatosEjemplo)
	{
		_logger = logger;
		mapeoDatosGenerador = _mapeoDatosEjemplo!;
	}

	[HttpPost("pdfFacturacion")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<string>> pdfFacturacion(
		[FromBody] DatosFactura factura)
	{
		string facturaBase64 = "";

		try
		{
			FacturaDocumento facturaD = new FacturaDocumento();
			await Task.Factory.StartNew(() =>
			{
				facturaBase64 = facturaD.GenerarDocumento(factura);
			});
		}
		catch (Exception e)
		{
			return null!;
		}
		return facturaBase64;
	}
}
