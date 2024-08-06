using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionClienteHL.Entidades.Consultas;

public class EstadosHumalabClienteResponse
{
	[Column("IdCatalogoDetalle")]
	[JsonProperty("IdCatalogoDetalle")]
	public int IdCatalogoDetalle { get; set; }

	[Column("Nombre")]
	[JsonProperty("Nombre")]
	public string Nombre { get; set; } = string.Empty;

	[Column("Valor")]
	[JsonProperty("Valor")]
	public string Valor { get; set; } = string.Empty;
}
