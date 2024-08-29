using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PedidosHL.Entidades.Operaciones;

public class RecogerPedidoRequest
{
	[Required]
	[JsonProperty("IdPedido")]
	public int IdPedido { get; set; }
	[Required]
	[JsonProperty("IdOperador")]
	public int IdOperador { get; set; }
	[Required]
	[JsonProperty("ObservacionOperador")]
	public string ObservacionOperador { get; set; } = string.Empty;	
}
