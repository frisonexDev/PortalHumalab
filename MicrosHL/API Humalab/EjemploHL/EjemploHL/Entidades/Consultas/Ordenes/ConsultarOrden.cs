using Newtonsoft.Json;

namespace ClienteHL.Entidades.Consultas.Ordenes;

public class ConsultarOrden
{
	[JsonProperty("OpcionBusqueda")]
	public int? OpcionBusqueda { get; set; }

	[JsonProperty("DatoBusqueda")]
	public string DatoBusqueda { get; set; } = string.Empty;

	[JsonProperty("IdOrden")]
	public int? IdOrden { get; set; }

	[JsonProperty("CodigoBarra")]
	public string CodigoBarra { get; set; } = string.Empty;

	[JsonProperty("IdUsuarioGalileo")]
	public int? IdUsuarioGalileo { get; set; }

	[JsonProperty("FechaInicio")]
	public string FechaInicio { get; set; } = string.Empty;

	[JsonProperty("FechaFin")]
	public string FechaFin { get; set; } = string.Empty;

	[JsonProperty("opcionEstado")]
	public string opcionEstado { get; set; } = string.Empty;

	[JsonProperty("UsuarioCreacion")]
	public int? UsuarioCreacion { get; set; }

	[JsonProperty("FechaCreacion")]
	public DateTime FechaCreacion { get; set; } = DateTime.Now;
}
