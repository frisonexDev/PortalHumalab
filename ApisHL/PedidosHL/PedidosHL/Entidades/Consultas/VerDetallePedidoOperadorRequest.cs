using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PedidosHL.Entidades.Consultas;

public class VerDetallePedidoOperadorRequest
{
	[Required]
	[JsonProperty("IdPedido")]
	public int IdPedido { get; set; }
}
