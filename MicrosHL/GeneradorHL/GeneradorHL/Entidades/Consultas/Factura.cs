using Newtonsoft.Json;

namespace GeneradorHL.Entidades.Consultas;

public class Factura
{
	[JsonProperty("IdOrden")]
	public string IdOrden { get; set; } = string.Empty;

	[JsonProperty("IdPruebaGalileo")]
	public string IdPruebaGalileo { get; set; } = string.Empty;

	[JsonProperty("Codigo")]
	public string Codigo { get; set; } = string.Empty;

	[JsonProperty("Nombre")]
	public string Nombre { get; set; } = string.Empty;

	[JsonProperty("Cantidad")]
	public string Cantidad { get; set; } = string.Empty;

	[JsonProperty("Precio")]
	public string Precio { get; set; } = string.Empty;

	[JsonProperty("Estado")]
	public string Estado { get; set; } = string.Empty;

	[JsonProperty("EstadoFactura")]
	public string EstadoFactura { get; set; } = string.Empty;
}
