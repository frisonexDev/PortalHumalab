using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionClienteHL.Entidades.Consultas;

public class PedidoHumalabResponse
{
	[Column("NumeroRemision")]
	[JsonProperty("NumeroRemision")]
	public string NumeroRemision { get; set; } = string.Empty;
}
