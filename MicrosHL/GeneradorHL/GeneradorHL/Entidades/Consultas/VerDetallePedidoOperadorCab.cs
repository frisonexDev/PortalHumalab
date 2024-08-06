using Newtonsoft.Json;

namespace GeneradorHL.Entidades.Consultas;

public class VerDetallePedidoOperadorCab
{
	[JsonProperty("IdPedido")]
	public int IdPedido { get; set; }

	[JsonProperty("NumeroRemision")]
	public string NumeroRemision { get; set; } = string.Empty;

	[JsonProperty("EstadoPedido")]
	public string EstadoPedido { get; set; } = string.Empty;

	[JsonProperty("Cliente")]
	public string Cliente { get; set; } = string.Empty;

	[JsonProperty("ObservacionCliente")]
	public string ObservacionCliente { get; set; } = string.Empty;

	[JsonProperty("Usuario")]
	public string Usuario { get; set; } = string.Empty;

	[JsonProperty("OperadorLogistico")]
	public string OperadorLogistico { get; set; } = string.Empty;

	[JsonProperty("CorreoCliente")]
	public string CorreoCliente { get; set; } = string.Empty;

	[JsonProperty("ObservacionOperador")]
	public string ObservacionOperador { get; set; } = string.Empty;

	[JsonProperty("TotalMuestras")]
	public int TotalMuestras { get; set; }

	[JsonProperty("MuestrasRecolectadas")]
	public int MuestrasRecolectadas { get; set; }

	[JsonProperty("Telefono")]
	public string Telefono { get; set; } = string.Empty;

	[JsonProperty("Paciente")]
	public string Paciente { get; set; } = string.Empty;
}
