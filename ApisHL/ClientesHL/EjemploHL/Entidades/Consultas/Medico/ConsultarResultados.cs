namespace ClienteHL.Entidades.Consultas.Medico;

public class ConsultarResultados
{
	public string FechaInicio { get; set; } = string.Empty;
	public string FechaFin { get; set; } = string.Empty;
	public int OpcionBusqueda { get; set; }
	public string DatoBusqueda { get; set; } = string.Empty;
	public int opcionEstado { get; set; }
	public string CodigoBarra { get; set; } = string.Empty;
	public int idLaboratorio { get; set; }
	public string Sedes { get; set; } = string.Empty;
}
