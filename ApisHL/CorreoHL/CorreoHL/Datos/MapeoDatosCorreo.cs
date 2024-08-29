using CorreoHL.Entidades.Correo;
using CorreoHL.Utils;
using Microsoft.Azure.Management.Sql.Fluent;
using System.Drawing;
using System.Net;
using System.Net.Mail;

namespace CorreoHL.Datos;

public interface IMapeoDatosCorreo
{
	Task<int> ClienteNuevo(ClienteNuevo datos);
	Task<int> PedidoCorreo(PedidoCorreo pedidoCorreo);
	Task<int> ElimPedidoCorreo(ElimPedidoCorreo pedidoCorreo);
}
public class MapeoDatosCorreo: IMapeoDatosCorreo
{
	
	#region constructor

	public MapeoDatosCorreo()			
	{
		Servidor.Host = valor.Host;
		Servidor.EnableSsl = valor.EnableSsl;
		NetworkCredential network = new NetworkCredential(valor.User, valor.Password);
		Servidor.UseDefaultCredentials = false;
		Servidor.Credentials = network;
		Servidor.Port = valor.Port;
	}
	#endregion

	#region Instancia de Clases

	ConvertirPlantilla html = new ConvertirPlantilla();
	SmtpClient Servidor = new SmtpClient();
	ConfiguracionSmtp valor = new ConfiguracionSmtp();
	MailMessage Correo;

	#endregion

	#region Implementacion de la Interfaz

	Task<int> IMapeoDatosCorreo.ClienteNuevo(ClienteNuevo datos)
	{
		return ClienteNuevo(datos);
	}

	Task<int> IMapeoDatosCorreo.PedidoCorreo(PedidoCorreo pedidoCorreo)
	{
		return PedidosCorreo(pedidoCorreo);
	}

	Task<int> IMapeoDatosCorreo.ElimPedidoCorreo(ElimPedidoCorreo pedidoCorreo)
	{
		return EliminarPedCorreo(pedidoCorreo);
	}

	#endregion


	#region acciones de la clase	

	private async Task<int> ClienteNuevo(ClienteNuevo datos)
	{
		try
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			string Plantilla = "~/Plantillas/ClienteNuevo.cshtml";
			//string Mensaje = await html.CuerpoCorreo(datos.Usuario, datos.Contrasena, datos.Url, Plantilla);
			string Mensaje = await html.CuerpoCorreo(datos.NomUsuario, datos.Contrasena, datos.Url, Plantilla);
			string Asunto = "Bienvenido HumaLab";

			using (Correo = new MailMessage(valor.User, datos.Usuario))
			{
				Correo.Subject = Asunto;
				Correo.Body = Mensaje;
				Correo.IsBodyHtml = true;
				Servidor.Send(Correo);
			}

			return 201;
		}
		catch (System.Exception)
		{

			return 400;
		}
	}

	private async Task<int> PedidosCorreo(PedidoCorreo pedidoCorreo)
	{
		try
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

			string Plantilla = "~/Plantillas/PedidoCorreo.cshtml";
			string Mensaje = await html.CuerpoCorreoPedido(pedidoCorreo, Plantilla);
			string Asunto = "HumaLab Pedido";

			using (Correo = new MailMessage(valor.User, pedidoCorreo.correoOperador))
			{
				Correo.Subject = Asunto;
				Correo.Body = Mensaje;
				Correo.IsBodyHtml = true;
				Servidor.Send(Correo);
			}

			return 201;
		}
		catch (System.Exception)
		{
			return 400;
		}
	}

	private async Task<int> EliminarPedCorreo(ElimPedidoCorreo pedidoCorreo)
	{
		try
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

			string Plantilla = "~/Plantillas/ElimPedidoCorreo.cshtml";
			string Mensaje = await html.CuerpoCorreoElimPedido(pedidoCorreo, Plantilla);
			string Asunto = "HumaLab Pedido";

			using (Correo = new MailMessage(valor.User, pedidoCorreo.correoOperador))
			{
				Correo.Subject = Asunto;
				Correo.Body = Mensaje;
				Correo.IsBodyHtml = true;
				Servidor.Send(Correo);
			}

			return 201;
		}
		catch (System.Exception)
		{
			return 400;
		}
	}

	#endregion
}
