using Newtonsoft.Json;

namespace GeneradorHL.Entidades.Consultas;

public class VerDetallePedidoOperadorDet
{
	[JsonProperty("Numero")]
	public int Numero { get; set; }

	[JsonProperty("CodigoBarra")]
	public string CodigoBarra { get; set; } = string.Empty;

	[JsonProperty("TipoMuestra")]
	public string TipoMuestra { get; set; } = string.Empty;

	[JsonProperty("FechaOrden")]
	public DateTime FechaOrden { get; set; } = DateTime.Now;

	[JsonProperty("Identificacion")]
	public string Identificacion { get; set; } = string.Empty;

	[JsonProperty("Retirado")]
	public bool Retirado { get; set; }

	[JsonProperty("Rechazado")]
	public bool Rechazado { get; set; }

	[JsonProperty("ObservacionOperador")]
	public string ObservacionOperador { get; set; } = string.Empty;

	[JsonProperty("EstadoMuestra")]
	public string EstadoMuestra { get; set; } = string.Empty;

	[JsonProperty("IdPruebaMuestra")]
	public int IdPruebaMuestra { get; set; }

	[JsonProperty("Paciente")]
	public string Paciente { get; set; } = string.Empty;
}
