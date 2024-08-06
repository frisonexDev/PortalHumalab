using Newtonsoft.Json;

namespace PedidosHL.Entidades.Consultas;

public class _ConsultarPedidoOpLogRequest
{
	[JsonProperty("Accion")]
	public string Accion { get; set; } = string.Empty;

	[JsonProperty("FechaDesde")]
	public DateTime FechaDesde { get; set; }

	[JsonProperty("FechaHasta")]
	public DateTime FechaHasta { get; set; }

	[JsonProperty("Cliente")]
	public string Cliente { get; set; } = string.Empty;

	[JsonProperty("EsIdentificacion")]
	public bool? EsIdentificacion { get; set; }

	[JsonProperty("IdOperadorLogistico")]
	public int? IdOperadorLogistico { get; set; }

	[JsonProperty("Estado")]
	public string Estado { get; set; } = string.Empty;

	[JsonProperty("IdPedido")]
	public int IdPedido { get; set; }

	[JsonProperty("ObservacionOperador")]
	public string ObservacionOperador { get; set; } = string.Empty;

	[JsonProperty("PedidosEntrega")]
	public string PedidosEntrega { get; set; } = string.Empty;
}
