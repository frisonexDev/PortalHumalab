using Newtonsoft.Json;

namespace ClienteHL.Entidades.Consultas.Ordenes;

public class PruebaMuestra
{
	[JsonProperty("IdMuestraGalileo")]
	public int IdMuestraGalileo { get; set; }

	[JsonProperty("IdPruebaGalileo")]
	public int IdPruebaGalileo { get; set; }

	[JsonProperty("CodigoBarra")]
	public string CodigoBarra { get; set; }

	[JsonProperty("UsuarioCreacion")]
	public int UsuarioCreacion { get; set; }

	[JsonProperty("FechaCreacion")]
	public DateTime FechaCreacion { get; set; } = DateTime.Now;
}
