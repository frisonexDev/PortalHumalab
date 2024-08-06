using Newtonsoft.Json;

namespace ClienteHL.Entidades.Consultas.Pedidos;

public class ConsultarPedido
{
	[JsonProperty("OpcionBusqueda")]
	public int OpcionBusqueda { get; set; }

	[JsonProperty("DatoBusqueda")]
	public string DatoBusqueda { get; set; } = string.Empty;

	[JsonProperty("IdUsuario")]
	public int IdUsuario { get; set; }

	[JsonProperty("FechaDesde")]
	public string FechaDesde { get; set; } = string.Empty;

	[JsonProperty("FechaHasta")]
	public string FechaHasta { get; set; } = string.Empty;

	[JsonProperty("numRemision")]
	public string numRemision { get; set; } = string.Empty;

}
