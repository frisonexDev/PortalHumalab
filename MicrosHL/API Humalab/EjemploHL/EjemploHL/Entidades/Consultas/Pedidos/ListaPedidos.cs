using Newtonsoft.Json;

namespace ClienteHL.Entidades.Consultas.Pedidos;

public class ListaPedidos
{
	[JsonProperty("IdPedido")]
	public int IdPedido { get; set; }

	[JsonProperty("NumeroRemision")]
	public string? NumeroRemision { get; set; }

	[JsonProperty("FechaCreacion")]
	public DateTime FechaCreacion { get; set; }

	[JsonProperty("TotalOrdenes")]
	public int TotalOrdenes { get; set; }

	[JsonProperty("TotalMuestras")]
	public int TotalMuestras { get; set; }

	[JsonProperty("TotalRetiradas")]
	public int TotalRetiradas { get; set; }

	[JsonProperty("FechaRetiro")]
	public DateTime FechaRetiro { get; set; }

	[JsonProperty("EstadoPedido")]
	public string? EstadoPedido { get; set; }

	[JsonProperty("UsuarioCreacion")]
	public int UsuarioCreacion { get; set; }
}
