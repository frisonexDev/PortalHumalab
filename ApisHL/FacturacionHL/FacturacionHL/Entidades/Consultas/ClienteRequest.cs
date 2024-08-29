using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacturacionHL.Entidades.Consultas;

public class ClienteRequest
{
	[Column("Ruc")]
	[JsonProperty("Ruc")]
	public string? Ruc { get; set; }

	[Column("Estado")]
	[JsonProperty("Estado")]
	public string? Estado { get; set; }

	[Column("Desde")]
	[JsonProperty("Desde")]
	public string? Desde { get; set; }

	[Column("Hasta")]
	[JsonProperty("Hasta")]
	public string? Hasta { get; set; }

	[Column("Tipo")]
	[JsonProperty("Tipo")]
	public string? Tipo { get; set; }

	[Column("Valor")]
	[JsonProperty("Valor")]
	public string? Valor { get; set; }
}
