using GeneradorHL.Entidades.Consultas;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using System.Drawing;
using System.Runtime.Versioning;
using System.Text;
using VetCV.HtmlRendererCore.PdfSharpCore;
using ZXing;
using ZXing.Windows.Compatibility;

namespace GeneradorHL.Documentos.CodigoBarras;

public class CodigoBarrasDocumento
{
	public string LeerCodigoBarrasHtmlFinal(List<CodigoBarrasPdf> BarrasCodigo, string htmlcontent)
	{
		//List<Image> codImages = new List<Image>();
		List<CodigoBarrasPdf> codigosNuevos = new List<CodigoBarrasPdf>();

		string[] codigos = { };
		var codigoBase64 = string.Empty;
		string base64Pdf = string.Empty;
		var modificadoHtml = string.Empty;
		string fileName = string.Empty;

		try
		{
			var data = new PdfSharpCore.Pdf.PdfDocument();
			//Genera código de barras
			foreach (var barcode in (dynamic)BarrasCodigo)
			{
				BarcodeWriter barcodeWriter = new BarcodeWriter()!;
				barcodeWriter.Format = BarcodeFormat.CODE_128;
				barcodeWriter.Options = new ZXing.Common.EncodingOptions
				{
					Width = 400, // Ancho total código de barras
					Height = 100, // Altura total código de barras
					Margin = 10, // Margen alrededor código de barras
					PureBarcode = false //mostrar el número del código
				};

				var codigoBitmap = barcodeWriter.Write(barcode.Codigo);

#pragma warning disable CA1416
				codigoBase64 = ImagenBase64(codigoBitmap);

				codigosNuevos.Add(new CodigoBarrasPdf
				{
					Nombre = barcode.Nombre,
					Codigo = codigoBase64,
					NombrePaciente = barcode.NombrePaciente,
					IdentiPaciente = barcode.IdentiPaciente,
					muestraGalileo = barcode.muestraGalileo
				});
			}

			StringBuilder finalCodigos = new StringBuilder();
			foreach (var valores in codigosNuevos)
			{
				finalCodigos.Append($"<p style=\"text-align:center; font-size: 20px;\">{valores.Nombre}</p>");
				finalCodigos.Append($"<p style=\"text-align:center; font-size: 12px;\">{valores.NombrePaciente}</p>");
				finalCodigos.Append($"<p style=\"text-align:center; font-size: 12px;\">{valores.IdentiPaciente}</p>");
				finalCodigos.Append($"<center><img src='data:image/png;base64,{valores.Codigo}'/></center>");
				finalCodigos.Append($"<p style=\"text-align:center; font-size: 10px;\">{valores.muestraGalileo}</p><br />");
			}

			//Reemplaza datos en el PDF
			htmlcontent = htmlcontent.Replace("{ImagesPlaceholder}", finalCodigos.ToString());

			//Dimensiones PDF
			var pageSize = new XSize();
			pageSize.Width = 400;
			pageSize.Height = 255;

			PdfGenerateConfig config = new PdfGenerateConfig();
			config.ManualPageSize = pageSize;
			config.PageOrientation = PageOrientation.Portrait;
			config.MarginLeft = 3;
			config.MarginRight = 5;

			PdfGenerator.AddPdfPages(data, htmlcontent, config);

			using (MemoryStream ms = new MemoryStream())
			{
				data.Save(ms, false);
				ms.Seek(0, SeekOrigin.Begin);

				byte[] response = ms.ToArray();

				base64Pdf = Convert.ToBase64String(response);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error " + ex.Message);
		}

		return base64Pdf;
	}

	//soporte para plataforma LINUX
	[SupportedOSPlatform("linux")]
	public string ImagenBase64(Image image)
	{
		using (MemoryStream ms = new MemoryStream())
		{
			image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
			byte[] imageBytes = ms.ToArray();
			return System.Convert.ToBase64String(imageBytes);
		}
	}
}
