using Newtonsoft.Json;

namespace ClienteHL.Entidades.Consultas.Ordenes;

public class Paciente
{
	[JsonProperty("Identificacion")]
	public string Identificacion { get; set; } = string.Empty;

	[JsonProperty("Nombres")]
	public string Nombres { get; set; } = string.Empty;

	[JsonProperty("Apellidos")]
	public string Apellidos { get; set; } = string.Empty;

	[JsonProperty("Genero")]
	public bool Genero { get; set; }

	[JsonProperty("FechaNacimiento")]
	public DateTime FechaNacimiento { get; set; }

	[JsonProperty("Edad")]
	public string Edad { get; set; } = string.Empty;

	[JsonProperty("Telefono")]
	public string Telefono { get; set; } = string.Empty;

	[JsonProperty("Email")]
	public string Email { get; set; } = string.Empty;

	[JsonProperty("UsuarioCreacion")]
	public int UsuarioCreacion { get; set; }

	[JsonProperty("FechaCreacion")]
	public DateTime FechaCreacion { get; set; } = DateTime.Now;
}
