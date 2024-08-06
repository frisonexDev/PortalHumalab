using Newtonsoft.Json;

namespace GestionClienteHL.Entidades.Consultas;

public class PedidosHumalabClienteRequest
{
	[JsonProperty("idCliente")]
	public int idCliente { get; set; }
}
