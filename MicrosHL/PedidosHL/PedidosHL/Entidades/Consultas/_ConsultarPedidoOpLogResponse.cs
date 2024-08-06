using Newtonsoft.Json;

namespace PedidosHL.Entidades.Consultas;

public class _ConsultarPedidoOpLogResponse
{
	[JsonProperty("idPedido")]
	public int IdPedido { get; set; }

	[JsonProperty("idCliente")]
	public int IdCliente { get; set; }

	[JsonProperty("Cliente")]
	public string Cliente { get; set; } = string.Empty;

	[JsonProperty("numeroRemision")]
	public string NumeroRemision { get; set; } = string.Empty;

	[JsonProperty("fechaCreacion")]
	public DateTime? FechaCreacion { get; set; }

	[JsonProperty("totalOrdenes")]
	public int TotalOrdenes { get; set; }

	[JsonProperty("totalMuestras")]
	public int TotalMuestras { get; set; }

	[JsonProperty("totalRetiradas")]
	public int TotalRetiradas { get; set; }

	[JsonProperty("fechaRetiro")]
	public DateTime? FechaRetiro { get; set; }

	[JsonProperty("estadoPedido")]
	public string EstadoPedido { get; set; } = string.Empty;
}
