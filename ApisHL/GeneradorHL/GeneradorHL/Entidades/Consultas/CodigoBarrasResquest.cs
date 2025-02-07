using Newtonsoft.Json;

namespace GeneradorHL.Entidades.Consultas;

public class CodigoBarrasResquest
{
	[JsonProperty("IdMuestraGalileo")]
	public int? IdMuestraGalileo { get; set; }

	[JsonProperty("IdOrden")]
	public int? IdOrden { get; set; }

	[JsonProperty("NombreMuestra")]
	public string? NombreMuestra { get; set; }

	[JsonProperty("CodigoBarra")]
	public string? CodigoBarra { get; set; }

	[JsonProperty("EstadoMuestra")]
	public int? EstadoMuestra { get; set; }

	[JsonProperty("UsuarioCreacion")]
	public int? UsuarioCreacion { get; set; }

	[JsonProperty("FechaCreacion")]
	public DateTime FechaCreacion { get; set; } = DateTime.Now;

    [JsonProperty("IdMuestra")]
    public int? IdMuestra { get; set; }
}
