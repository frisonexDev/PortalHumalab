using Newtonsoft.Json;

namespace PedidosHL.Entidades.Consultas;

public class OrdenLaboratoristaResponse
{
	[JsonProperty("IdOrden")]
	public int IdOrden { get; set; }

	[JsonProperty("IdPedido")]
	public int IdPedido { get; set; }

	[JsonProperty("CodigoBarra")]
	public string CodigoBarra { get; set; } = string.Empty;

	[JsonProperty("FechaCreacion")]
	public DateTime? FechaCreacion { get; set; }

	[JsonProperty("UsuarioOperador")]
	public string UsuarioOperador { get; set; } = string.Empty;

	[JsonProperty("EstadoPedido")]
	public string EstadoPedido { get; set; } = string.Empty;

	[JsonProperty("ObservacionMuestras")]
	public string ObservacionMuestras { get; set; } = string.Empty;
}
