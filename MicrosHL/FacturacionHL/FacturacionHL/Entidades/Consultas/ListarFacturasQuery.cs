using Newtonsoft.Json;

namespace FacturacionHL.Entidades.Consultas;

public class ListarFacturasQuery
{
	[JsonProperty("IdOrden", NullValueHandling = NullValueHandling.Ignore)]
	public string? IdOrden { get; set; }

	[JsonProperty("IdPruebaGalileo", NullValueHandling = NullValueHandling.Ignore)]
	public string? IdPruebaGalileo { get; set; }

	[JsonProperty(nameof(Codigo), NullValueHandling = NullValueHandling.Ignore)]
	public string? Codigo { get; set; } = string.Empty;

	[JsonProperty(nameof(Nombre), NullValueHandling = NullValueHandling.Ignore)]
	public string? Nombre { get; set; } = string.Empty;

	[JsonProperty("Cantidad", NullValueHandling = NullValueHandling.Ignore)]
	public string? Cantidad { get; set; }

	[JsonProperty("Precio", NullValueHandling = NullValueHandling.Ignore)]
	public string? Precio { get; set; }

	[JsonProperty("Estado", NullValueHandling = NullValueHandling.Ignore)]
	public string? Estado { get; set; }

	[JsonProperty("EstadoFactura", NullValueHandling = NullValueHandling.Ignore)]
	public string? EstadoFactura { get; set; }
}
