using Newtonsoft.Json;

namespace GestionClienteHL.Entidades.Consultas;

public class MuestrasHumalabClienteRequest
{
	[JsonProperty("idCliente")]
	public int idCliente { get; set; }
}
