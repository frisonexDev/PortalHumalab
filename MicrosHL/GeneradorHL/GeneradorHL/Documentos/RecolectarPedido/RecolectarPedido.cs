using GeneradorHL.Entidades.Consultas;
using GeneradorHL.Utils;
using HtmlAgilityPack;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using System.Text;
using VetCV.HtmlRendererCore.PdfSharpCore;

namespace GeneradorHL.Documentos.RecolectarPedido;

public class RecolectarPedido
{
	public string GenerarDocumento(VerDetallePedidoOperadorResponse pedidoOpe)
	{
		StringBuilder pdfPedidos = new StringBuilder();

		string rutaHtml = StringHandler.rutaHtmlPedidos;
		string htmlcontent = "";
		string base64Pdf = "";

		HtmlDocument htmlDocument = new HtmlDocument();
		try
		{
			htmlcontent = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\">\r\n<head>\r\n    <meta charset=\"UTF-8\" />\r\n    <meta content=\"width=device-width, initial-scale=1\" name=\"viewport\" />\r\n    <meta name=\"x-apple-disable-message-reformatting\" />\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />\r\n    <meta content=\"telephone=no\" name=\"format-detection\" />\r\n    <title>Pedidos Cliente</title>\r\n\r\n    <style type=\"text/css\">\r\n        body {\r\n            background-color: white;\r\n        }\r\n\r\n        h1 {\r\n            text-align: center;\r\n        }\r\n\r\n        .border {\r\n            border-collapse: collapse;\r\n            width: 100%;\r\n            margin-bottom: 20px;\r\n        }\r\n\r\n            .border th, .border td {\r\n                padding: 5px;\r\n                border: 2px solid black;\r\n            }\r\n\r\n        .header {\r\n            display: flex;\r\n            justify-content: space-between;\r\n            margin-bottom: 20px;\r\n        }\r\n\r\n            .header div {\r\n                flex-basis: 48%;\r\n            }\r\n\r\n            .header strong {\r\n                display: inline-block;\r\n                width: 150px;\r\n            }\r\n\r\n        .summary {\r\n            text-align: left;\r\n            width: 100%;\r\n            display: flex;\r\n            justify-content: space-between;\r\n            margin-bottom: 20px;\r\n        }\r\n\r\n            .summary strong {\r\n                flex-basis: 50%;\r\n            }\r\n    </style>\r\n</head>\r\n<body style=\"width:100%;height:100%;padding:0;Margin:0\">\r\n    <br />\r\n    <div>\r\n        <img class=\"adapt-img\" src=\"https://slynox.stripocdn.email/content/guids/CABINET_6c99b0fb926c16c619692950ec20f07b97ec51ad8db74166634208aa62d01eed/images/humalab.PNG\" alt=\"\" width=\"150\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\" />\r\n    </div>\r\n\r\n\r\n    <table style=\"width:100%\">\r\n        <tr>\r\n            <td style=\"width:50%\">\r\n                <p><strong id=\"cliente\">asda</strong></p>\r\n                <p><strong id=\"usuario\">asd</strong></p>\r\n                <p><strong id=\"contacto\">asd</strong></p>\r\n                <p><strong id=\"correo\">asd</strong></p>\r\n                <p><strong id=\"obser\">asd</strong></p>\r\n            </td>\r\n\r\n            <td style=\"width:50%\">\r\n                <p><strong id=\"remision\">asd</strong></p>\r\n                <p><strong id=\"operador\">asd</strong></p>\r\n                <!--<p><strong id=\"contactoOp\">asd</strong></p>-->\r\n                <p><strong id=\"correoOp\">asd</strong></p>\r\n                <p><strong id=\"obserOp\">asd</strong></p>\r\n            </td>\r\n        </tr>\r\n    </table>\r\n\r\n    <div class=\"es-wrapper-color\" style=\"background-color:#FDFEFE\">\r\n        <table class=\"border\" style=\"width:100%\">\r\n            <thead>\r\n                <tr>\r\n                    <th>Paciente</th>\r\n                    <th>Muestra</th>\r\n                    <th>Fecha</th>\r\n                    <th>Usuario</th>\r\n                    <th>Retirado</th>\r\n                    <th>Rechazado</th>\r\n                    <th>Comentario</th>\r\n                    <th>Estado</th>\r\n                </tr>\r\n            </thead>\r\n\r\n            <tbody id=\"tablaPedidosDatos\">\r\n            </tbody>\r\n\r\n        </table>\r\n\r\n        <p><strong id=\"totalMuestras\"></strong></p>\r\n        <p><strong id=\"muestrasReco\"></strong></p>\r\n\r\n    </div>\r\n\r\n</body>\r\n</html>";

			htmlDocument.LoadHtml(htmlcontent);

			//cabecera
			htmlDocument.GetElementbyId("cliente").InnerHtml = "Cliente: " + pedidoOpe.Pedido.Cliente;
			htmlDocument.GetElementbyId("usuario").InnerHtml = "Responsable: " + pedidoOpe.Pedido.Usuario;
			htmlDocument.GetElementbyId("contacto").InnerHtml = "Contacto responsable: " + pedidoOpe.Pedido.Telefono;
			htmlDocument.GetElementbyId("correo").InnerHtml = "Correo responsable: " + pedidoOpe.Pedido.CorreoCliente;
			htmlDocument.GetElementbyId("obser").InnerHtml = "Observación responsable: " + pedidoOpe.Pedido.ObservacionCliente;

			htmlDocument.GetElementbyId("remision").InnerHtml = "N° de remisión: " + pedidoOpe.Pedido.NumeroRemision;
			htmlDocument.GetElementbyId("operador").InnerHtml = "Operador logístico: " + pedidoOpe.Pedido.OperadorLogistico;
			//htmlDocument.GetElementbyId("contactoOp").InnerHtml = "Contacto operador logístico: " + pedidoOpe.Pedido.CorreoCliente;
			htmlDocument.GetElementbyId("correoOp").InnerHtml = "Correo operador logístico: " + pedidoOpe.Pedido.CorreoCliente;
			htmlDocument.GetElementbyId("obserOp").InnerHtml = "Observación operador logístico: " + pedidoOpe.Pedido.ObservacionOperador;

			//datos de la tabla
			var tablaPedidos = htmlDocument.GetElementbyId("tablaPedidosDatos");

			foreach (var listaTabla in pedidoOpe.Muestras)
			{
				var filaTabla = htmlDocument.CreateElement("tr");
				tablaPedidos.AppendChild(filaTabla);

				var Paciente = htmlDocument.CreateElement("td");
				Paciente.InnerHtml = listaTabla.Paciente != null ? listaTabla.Paciente.ToString() : "";
				filaTabla.AppendChild(Paciente);

				var TipoMuestra = htmlDocument.CreateElement("td");
				TipoMuestra.InnerHtml = listaTabla.TipoMuestra != null ? listaTabla.CodigoBarra.ToString() : "";
				filaTabla.AppendChild(TipoMuestra);

				var Numero = htmlDocument.CreateElement("td");
				Numero.InnerHtml = listaTabla.FechaOrden.ToString("yyyy-MM-dd");
				filaTabla.AppendChild(Numero);

				var Identificacion = htmlDocument.CreateElement("td");
				Identificacion.InnerHtml = listaTabla.Identificacion != null ? listaTabla.Identificacion.ToString() : "";
				filaTabla.AppendChild(Identificacion);

				var IdPruebaReti = htmlDocument.CreateElement("td");
				IdPruebaReti.InnerHtml = listaTabla.Retirado == true ? "Sí" : "No";
				filaTabla.AppendChild(IdPruebaReti);

				var IdPruebaRecha = htmlDocument.CreateElement("td");
				IdPruebaRecha.InnerHtml = listaTabla.Rechazado == true ? "Sí" : "No";
				filaTabla.AppendChild(IdPruebaRecha);

				var ObservacionOperador = htmlDocument.CreateElement("td");
				ObservacionOperador.InnerHtml = listaTabla.ObservacionOperador != null ? listaTabla.ObservacionOperador.ToString() : "";
				filaTabla.AppendChild(ObservacionOperador);

				var EstadoMuestra = htmlDocument.CreateElement("td");
				EstadoMuestra.InnerHtml = listaTabla.EstadoMuestra != null ? listaTabla.EstadoMuestra.ToString() : "";
				filaTabla.AppendChild(EstadoMuestra);
			}

			htmlDocument.GetElementbyId("totalMuestras").InnerHtml = "Total de muestras: " + pedidoOpe.Pedido.TotalMuestras;
			htmlDocument.GetElementbyId("muestrasReco").InnerHtml = "Muestras recolectadas: " + pedidoOpe.Pedido.MuestrasRecolectadas;

			//arma todo el HTML
			var finalHtml = htmlDocument.DocumentNode.OuterHtml;

			var data = new PdfDocument();

			PdfGenerator.AddPdfPages(data, finalHtml, PageSize.A4);
			using (MemoryStream ms = new MemoryStream())
			{
				data.Save(ms, false);
				ms.Seek(0, SeekOrigin.Begin);

				byte[] response = ms.ToArray();

				base64Pdf = Convert.ToBase64String(response);
			}

			//using (StreamReader reader = new StreamReader(rutaHtml))
			//{
			//	htmlcontent = reader.ReadToEnd();

			//	htmlDocument.LoadHtml(htmlcontent);

			//	//cabecera
			//	htmlDocument.GetElementbyId("cliente").InnerHtml = "Cliente: " + pedidoOpe.Pedido.Cliente;
			//	htmlDocument.GetElementbyId("usuario").InnerHtml = "Responsable: " + pedidoOpe.Pedido.Usuario;
			//	htmlDocument.GetElementbyId("contacto").InnerHtml = "Contacto responsable: " + pedidoOpe.Pedido.Telefono;
			//	htmlDocument.GetElementbyId("correo").InnerHtml = "Correo responsable: " + pedidoOpe.Pedido.CorreoCliente;
			//	htmlDocument.GetElementbyId("obser").InnerHtml = "Observación responsable: " + pedidoOpe.Pedido.ObservacionCliente;

			//	htmlDocument.GetElementbyId("remision").InnerHtml = "N° de remisión: " + pedidoOpe.Pedido.NumeroRemision;
			//	htmlDocument.GetElementbyId("operador").InnerHtml = "Operador logístico: " + pedidoOpe.Pedido.OperadorLogistico;
			//	//htmlDocument.GetElementbyId("contactoOp").InnerHtml = "Contacto operador logístico: " + pedidoOpe.Pedido.CorreoCliente;
			//	htmlDocument.GetElementbyId("correoOp").InnerHtml = "Correo operador logístico: " + pedidoOpe.Pedido.CorreoCliente;
			//	htmlDocument.GetElementbyId("obserOp").InnerHtml = "Observación operador logístico: " + pedidoOpe.Pedido.ObservacionOperador;

			//	//datos de la tabla
			//	var tablaPedidos = htmlDocument.GetElementbyId("tablaPedidosDatos");

			//	foreach (var listaTabla in pedidoOpe.Muestras)
			//	{
			//		var filaTabla = htmlDocument.CreateElement("tr");
			//		tablaPedidos.AppendChild(filaTabla);

			//		var Paciente = htmlDocument.CreateElement("td");
			//		Paciente.InnerHtml = listaTabla.Paciente != null ? listaTabla.Paciente.ToString() : "";
			//		filaTabla.AppendChild(Paciente);

			//                 var TipoMuestra = htmlDocument.CreateElement("td");
			//		TipoMuestra.InnerHtml = listaTabla.TipoMuestra != null ? listaTabla.CodigoBarra.ToString() : "";
			//		filaTabla.AppendChild(TipoMuestra);

			//		var Numero = htmlDocument.CreateElement("td");
			//		Numero.InnerHtml = listaTabla.FechaOrden.ToString("yyyy-MM-dd");
			//		filaTabla.AppendChild(Numero);

			//		var Identificacion = htmlDocument.CreateElement("td");
			//		Identificacion.InnerHtml = listaTabla.Identificacion != null ? listaTabla.Identificacion.ToString() : "";
			//		filaTabla.AppendChild(Identificacion);

			//		var IdPruebaReti = htmlDocument.CreateElement("td");
			//		IdPruebaReti.InnerHtml = listaTabla.Retirado == true ? "Sí" : "No";
			//		filaTabla.AppendChild(IdPruebaReti);

			//		var IdPruebaRecha = htmlDocument.CreateElement("td");
			//		IdPruebaRecha.InnerHtml = listaTabla.Rechazado == true ? "Sí" : "No";
			//		filaTabla.AppendChild(IdPruebaRecha);

			//		var ObservacionOperador = htmlDocument.CreateElement("td");
			//		ObservacionOperador.InnerHtml = listaTabla.ObservacionOperador != null ? listaTabla.ObservacionOperador.ToString() : "";
			//		filaTabla.AppendChild(ObservacionOperador);

			//		var EstadoMuestra = htmlDocument.CreateElement("td");
			//		EstadoMuestra.InnerHtml = listaTabla.EstadoMuestra != null ? listaTabla.EstadoMuestra.ToString() : "";
			//		filaTabla.AppendChild(EstadoMuestra);
			//	}

			//	htmlDocument.GetElementbyId("totalMuestras").InnerHtml = "Total de muestras: " + pedidoOpe.Pedido.TotalMuestras;
			//	htmlDocument.GetElementbyId("muestrasReco").InnerHtml = "Muestras recolectadas: " + pedidoOpe.Pedido.MuestrasRecolectadas;

			//	//arma todo el HTML
			//	var finalHtml = htmlDocument.DocumentNode.OuterHtml;

			//	var data = new PdfDocument();

			//	PdfGenerator.AddPdfPages(data, finalHtml, PageSize.A4);
			//	using (MemoryStream ms = new MemoryStream())
			//	{
			//		data.Save(ms, false);
			//		ms.Seek(0, SeekOrigin.Begin);

			//		byte[] response = ms.ToArray();

			//		base64Pdf = Convert.ToBase64String(response);
			//	}
			//}
		}
		catch (Exception ex)
		{
			return ex.Message.ToString();
		}

		return base64Pdf;
	}
}
