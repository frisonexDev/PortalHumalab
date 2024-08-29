using GeneradorHL.Entidades.Consultas;
using GeneradorHL.Utils;
using HtmlAgilityPack;
using System.Text;
using VetCV.HtmlRendererCore.PdfSharpCore;

namespace GeneradorHL.Documentos.Facturacion;

public class FacturaDocumento
{
	public string GenerarDocumento(DatosFactura factura)
	{
		string base64Pdf = "";
		StringBuilder pdfFactura = new StringBuilder();

		string rutaHtml = StringHandler.rutaHtmlFactura;
		string htmlcontent = "";

		HtmlDocument htmlDocument = new HtmlDocument();


		try
		{
			htmlcontent = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\">\r\n<head>\r\n    <meta charset=\"UTF-8\" />\r\n    <meta content=\"width=device-width, initial-scale=1\" name=\"viewport\" />\r\n    <meta name=\"x-apple-disable-message-reformatting\" />\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />\r\n    <meta content=\"telephone=no\" name=\"format-detection\" />\r\n    <title>Factura Cliente</title>\r\n\r\n    <style type=\"text/css\">\r\n        body {\r\n            font-size: 10px;\r\n        }\r\n\r\n        .tableheader {\r\n            border: silver 1px solid;\r\n            padding: 5px;\r\n        }\r\n\r\n        .tablePruebas tr td {\r\n            border: silver 1px solid;\r\n            padding: 10px;\r\n        }\r\n\r\n        .cuadrado {\r\n            font-size: 24px;\r\n            border: solid 1px black;\r\n            width: 200px;\r\n            height: 10px;\r\n            text-align: center;\r\n            font-weight: bold;\r\n        }\r\n\r\n        #total {\r\n            display: flex;\r\n            flex-direction: column;\r\n            align-items: end;\r\n        }\r\n\r\n        p {\r\n            color: black;\r\n        }\r\n\r\n        #datosFactura {\r\n            width: 18%;\r\n            padding-top: 11px;\r\n        }\r\n    </style>\r\n</head>\r\n<body>\r\n    <br />\r\n    <div>\r\n        <img class=\"adapt-img\" src=\"https://slynox.stripocdn.email/content/guids/CABINET_6c99b0fb926c16c619692950ec20f07b97ec51ad8db74166634208aa62d01eed/images/humalab.PNG\" alt=\"\" width=\"150\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\" />\r\n    </div>\r\n\r\n\r\n    <table style=\"width:100%\">\r\n        <tr>\r\n            <td style=\"width:40%\">\r\n                <h2>HUMALAB S.A</h2>\r\n                <h3>HUMALAB S.A</h3>\r\n                <h4>Matriz: AV.6 DE DICIEMBRE N3-110 Y WHYMPER</h4>\r\n                <h5>Sucursal: URB. CIUDAD COLON, EDIFICIO METROPARK LOCAL 9</h5>\r\n                <h5>Contribuyente Especial Nro.</h5>\r\n                <h5>Obligado a llevar contabilidad: NO</h5>\r\n            </td>\r\n\r\n\r\n\r\n            <td style=\"width:50%\">\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n                <div id=\"estadoFac\" class=\"cuadrado\"></div>\r\n\r\n\r\n\r\n                <div>\r\n                    <h5>Fecha y Hora de Autorización : </h5><span id=\"fechaAut\"></span>\r\n\r\n                </div>  <div>\r\n                    <h5>Ambiente: PRODUCCIÓN</h5>\r\n                </div><div>\r\n                    <h5>Tipo de Emisión: NORMAL</h5>\r\n                </div>\r\n\r\n\r\n\r\n\r\n\r\n            </td>\r\n        </tr>\r\n    </table>\r\n\r\n\r\n    <div class=\"es-wrapper-color\" style=\"background-color:#FDFEFE\">\r\n        <div style=\" width:100%\">\r\n            <table class=\"tableheader\" style=\"width:100%\">\r\n                <tr class=\"tableheader\">\r\n                    <td style=\"font-weight: bold;\">Razón Social/Nombres y Apellidos:</td>\r\n                    <td id=\"nombre\"></td>\r\n                    <td style=\"font-weight: bold;\">Fecha Emisión: </td>\r\n                    <td id=\"fecha\"></td>\r\n                </tr>\r\n                <tr class=\"tableheader\">\r\n                    <td style=\"font-weight: bold;\">RUC/CI:</td>\r\n                    <td id=\"ruc\"></td>\r\n                    <td style=\"font-weight: bold;\">Guía Remisión:</td>\r\n                    <td id=\"guia\"></td>\r\n\r\n                </tr>\r\n                <tr class=\"tableheader\">\r\n                    <td></td>\r\n                    <td></td>\r\n                    <td style=\"font-weight: bold;\">Moneda:</td>\r\n                    <td id=\"dolar\">DOLAR</td>\r\n                </tr>\r\n                <tr>\r\n                </tr>\r\n            </table>\r\n\r\n            <br />\r\n            <br />\r\n            <table id=\"tablaFactura\" class=\"tablePruebas\" class=\"tablePruebas\" style=\"width:100%;\">\r\n\r\n                <thead>\r\n                    <tr class=\"tablePruebas\" style=\" text-align: center; font-weight: bold;\">\r\n                        <td style=\"font-weight: bold;\"> Código </td>\r\n                        <td style=\"font-weight: bold;\"> Descripción </td>\r\n                        <td style=\"font-weight: bold;\"> Cantidad </td>\r\n                        <td style=\"font-weight: bold;\"> Precio Unitario </td>\r\n                        <td style=\"font-weight: bold;\"> Total </td>\r\n                    </tr>\r\n                </thead>\r\n\r\n                <tbody id=\"tablaFacturaDatos\"></tbody>\r\n\r\n                <tfoot>\r\n                    <tr>\r\n                        <td style=\"text-align: right; font-weight: bold;\" colspan=\"4\">Total</td>\r\n                        <td id=\"total\" style=\"text-align: right; font-weight: bold;\"></td>\r\n                    </tr>\r\n                </tfoot>\r\n            </table>\r\n            <br />\r\n\r\n            <h5>Total Ordenes : <span id=\"totalMuestra\"></span></h5>\r\n            <h5>Total Factura :<span id=\"detalleMuestras\"></span></h5>\r\n\r\n        </div>\r\n\r\n    </div>\r\n\r\n\r\n</body>\r\n</html>";

			htmlDocument.LoadHtml(htmlcontent);

			//cabecera tabla
			htmlDocument.GetElementbyId("nombre").InnerHtml = factura.cliente;
			htmlDocument.GetElementbyId("ruc").InnerHtml = factura.Ruc;
			htmlDocument.GetElementbyId("fecha").InnerHtml = factura.fecha;
			htmlDocument.GetElementbyId("estadoFac").InnerHtml = factura.estadoFactura;

			DateTime fechaProceso = DateTime.Now;
			string fechaFinal = fechaProceso.ToString();
			htmlDocument.GetElementbyId("fechaAut").InnerHtml = fechaFinal;

			//datos de la tabla
			var tablaFactura = htmlDocument.GetElementbyId("tablaFacturaDatos");

			if (tablaFactura == null)
			{
				tablaFactura = htmlDocument.CreateElement("table");
				tablaFactura.Id = "miTablaFactura";
				htmlDocument.DocumentNode.AppendChild(tablaFactura);
			}

			foreach (var listaTabla in factura.facturaList)
			{
				var filaTabla = htmlDocument.CreateElement("tr");
				tablaFactura.AppendChild(filaTabla);

				var celdaCodigo = htmlDocument.CreateElement("td");
				celdaCodigo.InnerHtml = listaTabla.Codigo.ToString();
				filaTabla.AppendChild(celdaCodigo);

				var celdaDescripcion = htmlDocument.CreateElement("td");
				celdaDescripcion.InnerHtml = listaTabla.Nombre.ToString();
				filaTabla.AppendChild(celdaDescripcion);

				var celdaCantidad = htmlDocument.CreateElement("td");
				celdaCantidad.InnerHtml = listaTabla.Cantidad.ToString();
				filaTabla.AppendChild(celdaCantidad);

				var celdaPrecio = htmlDocument.CreateElement("td");
				celdaPrecio.InnerHtml = listaTabla.Precio.ToString();
				filaTabla.AppendChild(celdaPrecio);

				var celdaTotal = htmlDocument.CreateElement("td");
				celdaTotal.InnerHtml = listaTabla.Precio.ToString();
				filaTabla.AppendChild(celdaTotal);
			}

			htmlDocument.GetElementbyId("total").InnerHtml = factura.totalFactura;
			htmlDocument.GetElementbyId("totalMuestra").InnerHtml = factura.totalMuestra;

			string numOrden = string.Join(", ", factura.OrdenList);
			htmlDocument.GetElementbyId("detalleMuestras").InnerHtml = numOrden;

			//arma todo el HTML
			var finalHtml = htmlDocument.DocumentNode.OuterHtml;

			var data = new PdfSharpCore.Pdf.PdfDocument();

			PdfGenerator.AddPdfPages(data, finalHtml, PdfSharpCore.PageSize.A4);
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
			return ex.Message.ToString();
		}

		return base64Pdf;
	}
}
