using Newtonsoft.Json;

namespace ClienteHL.Entidades;

public class ConsultarPaciente
{
	[JsonProperty("Identificacion")]
	public string Identificacion { get; set; } = string.Empty;

	[JsonProperty("UsuarioCreacion")]
	public int UsuarioCreacion { get; set; }

	[JsonProperty("FechaCreacion")]
	public DateTime FechaCreacion { get; set; } = DateTime.Now;
}
