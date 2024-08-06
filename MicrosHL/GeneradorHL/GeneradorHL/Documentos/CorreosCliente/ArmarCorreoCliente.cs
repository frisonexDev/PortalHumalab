using GeneradorHL.Entidades.Consultas;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using VetCV.HtmlRendererCore.PdfSharpCore;

namespace GeneradorHL.Documentos.CorreosCliente;

public class ArmarCorreoCliente
{
	public string CorreoClienteHtml(CorreoCliente correo, string htmlcontent)
	{
		string base64 = string.Empty;
		try
		{
			htmlcontent = htmlcontent.Replace("{password}", correo.Contrasena)
									 .Replace("{usuario}", correo.Usuario)
									 .Replace("{Url}", correo.Url);

			var data = new PdfDocument();

			PdfGenerator.AddPdfPages(data, htmlcontent, PageSize.A4);
			using (MemoryStream ms = new MemoryStream())
			{
				data.Save(ms, false);
				ms.Seek(0, SeekOrigin.Begin);

				byte[] response = ms.ToArray();

				base64 = Convert.ToBase64String(response);
			}

		}
		catch (Exception ex)
		{
			Console.WriteLine("Error " + ex.Message);
		}

		return base64;
	}
}
