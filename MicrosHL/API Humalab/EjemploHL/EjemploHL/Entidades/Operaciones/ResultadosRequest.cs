namespace ClienteHL.Entidades.Operaciones;

public class ResultadosRequest
{
	public int idLaboratorio { get; set; }
	public string Identificacion { get; set; } = string.Empty;
	public string NumeroOrden { get; set; } = string.Empty;
	public string Estado { get; set; } = string.Empty;
	public string Genero { get; set; } = string.Empty;
	public string FechaNacimiento { get; set; } = string.Empty;
	public string FechaIngreso { get; set; } = string.Empty;
	public string Usuario { get; set; } = string.Empty;
	public string NombrePaciente { get; set; } = string.Empty;
	public string idSede { get; set; } = string.Empty;
	public string nombreSede { get; set; } = string.Empty;
}
