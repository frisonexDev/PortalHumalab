using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClienteHL.Entidades.Consultas.CatalogoMaestro;

public class CatalogoTiposClientes
{
	[Column("IdCatalogo")]
	[JsonProperty("IdCatalogo")]
	public int IdCatalogo { get; set; }

	[Column("NombreCatalogo")]
	[JsonProperty("NombreCatalogo")]
	public string NombreCatalogo { get; set; } = string.Empty;

	[Column("ValorCatalogo")]
	[JsonProperty("ValorCatalogo")]
	public string ValorCatalogo { get; set; } = string.Empty;
}
