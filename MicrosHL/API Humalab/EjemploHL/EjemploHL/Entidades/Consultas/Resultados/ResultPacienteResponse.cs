using Newtonsoft.Json;

namespace ClienteHL.Entidades.Consultas.Resultados;

public class ResultPacienteResponse
{
	[JsonProperty("Cedula")]
	public string Cedula { get; set; } = string.Empty;

	[JsonProperty("NumeroOrden")]
	public string NumeroOrden { get; set; } = string.Empty;

	[JsonProperty("FechaIngreso")]
	public string FechaIngreso { get; set; } = string.Empty;

	[JsonProperty("Estado")]
	public string Estado { get; set; } = string.Empty;

	[JsonProperty("NombrePaciente")]
	public string NombrePaciente { get; set; } = string.Empty;

	[JsonProperty("IdLaboratorio")]
	public int IdLaboratorio { get; set; }
}
