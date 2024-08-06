using Razor.Templating.Core;

namespace CorreoHL.Entidades.Correo;

public class ConvertirPlantilla
{
	public ConvertirPlantilla() { }


	//Ruta Plantilla debe enviarse en este formato ~/Plantillas/NombrePlantilla.cshtml

	public async Task<string> CuerpoCorreo(string RutaPlantilla)
	{
		var html = await RazorTemplateEngine.RenderAsync(RutaPlantilla);

		return html;

	}

	public async Task<string> CuerpoCorreo(string Usuario, string Contrasena, string RutaPlantilla)
	{
		var html = await RazorTemplateEngine.RenderAsync(RutaPlantilla);

		html = html.Replace("{usuario}", Usuario);
		html = html.Replace("{password}", Contrasena);

		return html;

	}



	public async Task<string> CuerpoCorreo(string Usuario, string Contrasena, string Link, string RutaPlantilla)
	{
		var html = await RazorTemplateEngine.RenderAsync(RutaPlantilla);

		html = html.Replace("{usuario}", Usuario);
		html = html.Replace("{password}", Contrasena);
		html = html.Replace("{Url}", Link);

		return html;

	}

	public async Task<string> CuerpoCorreoPedido(PedidoCorreo pedido, string rutaPlantilla)
	{
		var html = await RazorTemplateEngine.RenderAsync(rutaPlantilla);

		var fechaCorreo = DateTime.Now.ToString();

		html = html.Replace("{fechaEmision}", fechaCorreo);
		html = html.Replace("{operador}", pedido.operador);
		html = html.Replace("{clienteNombre}", pedido.clienteNombre);
		html = html.Replace("{numRemision}", pedido.numRemision);
		html = html.Replace("{totalMuestras}", pedido.TotalMuestras.ToString());
		html = html.Replace("{callePrincipal}", pedido.direccion);
		html = html.Replace("{calleSecundaria}", pedido.direccion);
		html = html.Replace("{telefonoCliente}", pedido.telefonoCliente);
		html = html.Replace("{correoCliente}", pedido.correoCliente);

		return html;
	}

	public async Task<string> CuerpoCorreoElimPedido(ElimPedidoCorreo pedido, string rutaPlantilla)
	{
		var html = await RazorTemplateEngine.RenderAsync(rutaPlantilla);

		var fechaCorreo = DateTime.Now.ToString();

		html = html.Replace("{fechaEmision}", fechaCorreo);
		html = html.Replace("{operador}", pedido.operador);
		html = html.Replace("{clienteNombre}", pedido.clienteNombre);
		html = html.Replace("{numRemision}", pedido.numRemision);
		html = html.Replace("{telefonoCliente}", pedido.telefonoCliente);
		html = html.Replace("{correoCliente}", pedido.correoCliente);
		html = html.Replace("{callePrincipal}", pedido.direccion);
		html = html.Replace("{observacion}", pedido.observacion);

		return html;
	}
}
