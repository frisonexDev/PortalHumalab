using Newtonsoft.Json;

namespace ClienteHL.Entidades.Consultas.Ordenes;

public class LogObservaciones
{
	[JsonProperty("Orden")]
	public string Orden { get; set; } = string.Empty;

	[JsonProperty("ObservacionOrden")]
	public string ObservacionOrden { get; set; } = string.Empty;

	[JsonProperty("Muestra")]
	public string Muestra { get; set; } = string.Empty;

	[JsonProperty("ObservacionMuestra")]
	public string ObservacionMuestra { get; set; } = string.Empty;


	[JsonProperty("Usuario")]
	public string Usuario { get; set; } = string.Empty;

	[JsonProperty("Fecha")]
	public DateTime Fecha { get; set; }
}
