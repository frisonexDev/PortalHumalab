namespace PedidosHL.Entidades.Consultas;

public class OrdenCab
{
	public string Genero { get; set; } = string.Empty;
	public DateTime FechaNacimiento { get; set; }
	public int Edad { get; set; }
	public string Diagnostico { get; set; } = string.Empty;
	public string Medicamento { get; set; } = string.Empty;
	public string ObservacionOrden { get; set; } = string.Empty;
	public string ObservacionCliente { get; set; } = string.Empty;
	public string ObservacionOpLogistico { get; set; } = string.Empty;
	public string FechaEnvio { get; set; } = string.Empty;
	public string NombresPaciente { get; set; } = string.Empty;
	public string Identificacion { get; set; } = string.Empty;
}
