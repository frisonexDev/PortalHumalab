using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PedidosHL.Entidades.Operaciones;

public class EntregarPedidoRequest
{
	[Required]
	[JsonProperty("Pedidos")]
	public string Pedidos { get; set; } = string.Empty;	
	[Required]
	[JsonProperty("IdOperador")]
	public int IdOperador { get; set; }
	[Required]
	[JsonProperty("ObservacionOperador")]
	public string ObservacionOperador { get; set; } = string.Empty;
}
