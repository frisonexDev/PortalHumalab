using Newtonsoft.Json;

namespace GeneradorHL.Entidades.Consultas;

public class DatosFactura
{
	[JsonProperty("Ruc")]
	public string Ruc { get; set; } = string.Empty;

	[JsonProperty("cliente")]
	public string cliente { get; set; } = string.Empty;

	[JsonProperty("fecha")]
	public string fecha { get; set; } = string.Empty;

	[JsonProperty("numeroFactura")]
	public string numeroFactura { get; set; } = string.Empty;

	[JsonProperty("facturaList")]
	public List<Factura> facturaList { get; set; } = new List<Factura>();

	[JsonProperty("totalMuestra")]
	public string totalMuestra { get; set; } = string.Empty;

	[JsonProperty("totalFactura")]
	public string totalFactura { get; set; } = string.Empty;

	[JsonProperty("estadoFactura")]
	public string estadoFactura { get; set; } = string.Empty;

	[JsonProperty("OrdenList")]
	public List<string> OrdenList { get; set; }
}
