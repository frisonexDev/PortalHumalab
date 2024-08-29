namespace GestionClienteHL.Entidades.Consultas;

public class PedidoOrdenHumalabResponse
{
	public int totalOrdenes { get; set; }
	public List<DiagnosticoPedido> Diagnostico { get; set; }
}

public class DiagnosticoPedido
{
	public string Nombre { get; set; } = string.Empty;
}