using Newtonsoft.Json;

namespace ClienteHL.Entidades.Consultas.Ordenes;

public class ListarOrden
{
	[JsonProperty("NOrden")]
	public int NOrden { get; set; }

	[JsonProperty("CodigoBarra")]
	public string CodigoBarra { get; set; } = string.Empty;

	[JsonProperty("FechaIngreso")]
	public DateTime FechaIngreso { get; set; }

	[JsonProperty("NombrePaciente")]
	public string NombrePaciente { get; set; } = string.Empty;

	[JsonProperty("Precio")]
	public float Precio { get; set; }

	[JsonProperty("Estado")]
	public string Estado { get; set; } = string.Empty;

	[JsonProperty("Observacion")]
	public string Observacion { get; set; } = string.Empty;

	[JsonProperty("CodigoGalileo")]
	public string CodigoGalileo { get; set; } = string.Empty;
}
