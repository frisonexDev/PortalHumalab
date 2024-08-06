using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PedidosHL.Entidades.Operaciones;

public class CambiarPruebaRequest
{
	[Required]
	[JsonProperty("IdPrueba")]
	public int IdPrueba { get; set; }
	[Required]
	[JsonProperty("IdMuestra")]
	public int IdMuestra { get; set; }
	[Required]
	[JsonProperty("Rechaza")]
	public bool Rechaza { get; set; }
	[Required]
	[JsonProperty("Usuario")]
	public int Usuario { get; set; }
	[JsonProperty("Observacion")]
	public string Observacion { get; set; } = string.Empty;
}
