using GeneradorHL.Datos;
using GeneradorHL.Documentos.CorreosCliente;
using GeneradorHL.Entidades.Consultas;
using GeneradorHL.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeneradorHL.Controllers;

[ApiController]
[Route("api/Humalab/GeneradorHl")]
public class CorreoController : ControllerBase
{
	private readonly ILogger<CorreoController> _logger;
	private readonly IMapeoDatosGenerador mapeoDatosGenerador;

	public CorreoController(
		ILogger<CorreoController> logger,
		IMapeoDatosGenerador _mapeoDatosEjemplo)
	{
		_logger = logger;
		mapeoDatosGenerador = _mapeoDatosEjemplo!;
	}

	[HttpPost("pdfCorreoCliente")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<string>> pdfCorreoCliente(
		[FromBody] CorreoCliente cliente)
	{
		try
		{
			string codigoFinal = string.Empty;
			string htmlcontent = string.Empty;
			string rutaHtml = StringHandler.rutaHtmlCorreo;

			using (StreamReader reader = new StreamReader(rutaHtml))
			{
				htmlcontent = reader.ReadToEnd();

				ArmarCorreoCliente correo = new ArmarCorreoCliente();

				await Task.Factory.StartNew(() =>
				{
					codigoFinal = correo.CorreoClienteHtml(cliente, htmlcontent);
				});
			}

			return codigoFinal;
		}
		catch (Exception e)
		{
			return null;
		}
	}
}
