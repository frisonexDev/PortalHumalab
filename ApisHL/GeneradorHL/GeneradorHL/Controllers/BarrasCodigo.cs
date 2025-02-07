using GeneradorHL.Datos;
using GeneradorHL.Entidades.Consultas;
using GeneradorHL.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GeneradorHL.Controllers
{
	[ApiController]
	[Route("api/Humalab/GeneradorHl")]
	public class BarrasCodigo : ControllerBase
	{
		private readonly ILogger<BarrasCodigo> _logger;
		private readonly IMapeoDatosGenerador mapeoDatosGenerador;

		public BarrasCodigo(
			ILogger<BarrasCodigo> logger,
			IMapeoDatosGenerador _mapeoDatosEjemplo)
		{
			_logger = logger;
			mapeoDatosGenerador = _mapeoDatosEjemplo!;
		}

		[HttpPost("pdfetiquetas")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<CodigoBarrasPdf>>> pdfetiquetas(		
			[FromBody] List<CodigoBarrasResquest> grabarPeticion)
		{
			List<CodigoBarrasPdf> BarrasCodigo = new List<CodigoBarrasPdf>();

			try
			{
				string ruta = StringHandler.rutaHtmlCodigos;
				string htmlcontent = "";
				string cliente = string.Empty;
				ClienteEtiquetas clienteEtiquetas = new ClienteEtiquetas();

				string codigoFinal = string.Empty;

				foreach (var muestras in grabarPeticion)
				{					
					await Task.Factory.StartNew(() =>
					{
						clienteEtiquetas = mapeoDatosGenerador.ObtenerNombre(muestras.UsuarioCreacion!.Value, muestras.CodigoBarra!, muestras.IdMuestra!.Value);
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

				//CodigoBarrasDocumento codigo = new CodigoBarrasDocumento();
				//htmlcontent = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <meta charset=\"utf-8\" />\r\n    <script src=\"https://cdn.jsdelivr.net/npm/jsbarcode@3.11.5/dist/JsBarcode.all.min.js\"></script>\r\n    <style>\r\n        \r\n    </style>\r\n</head>\r\n<body>    \r\n    {ImagesPlaceholder}    \r\n</body>\r\n</html>";
				//codigoFinal = codigo.LeerCodigoBarrasHtmlFinal(BarrasCodigo, htmlcontent);

				//using (StreamReader reader = new StreamReader(ruta))
				//{
				//	htmlcontent = reader.ReadToEnd();

				//	CodigoBarrasDocumento codigo = new CodigoBarrasDocumento();

				//	codigoFinal = codigo.LeerCodigoBarrasHtmlFinal(BarrasCodigo, htmlcontent);
				//}

			}
			catch (Exception e)
			{
				return null;
			}

			return BarrasCodigo;
		}
	}
}