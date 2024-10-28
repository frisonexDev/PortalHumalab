using Newtonsoft.Json;

namespace ClienteHL.Entidades.Operaciones;

public class GrabarPedidosRequest
{
	[JsonProperty("IdPedido")]
	public int IdPedido { get; set; }

	[JsonProperty("IdOperador")]
	public int IdOperador { get; set; }

	[JsonProperty("DatosOrden")]
	public List<OrdenP> DatosOrden { get; set; }

	[JsonProperty("UsuarioOperador")]
	public string UsuarioOperador { get; set; } = string.Empty;

	[JsonProperty("NumeroRemision")]
	public string NumeroRemision { get; set; } = string.Empty;
	[JsonProperty("EstadoPedido")]
	public int EstadoPedido { get; set; }

	[JsonProperty("Observacion")]
	public string? Observacion { get; set; }

	[JsonProperty("UsuarioCreacion")]
	public int UsuarioCreacion { get; set; }

	[JsonProperty("FechaCreacion")]
	public DateTime FechaCreacion { get; set; } = DateTime.Now;

	[JsonProperty("IdClientePedido")]
	public int IdClientePedido { get; set; }
}

public class OrdenP
{
	public int IdOrden { get; set; }

}
