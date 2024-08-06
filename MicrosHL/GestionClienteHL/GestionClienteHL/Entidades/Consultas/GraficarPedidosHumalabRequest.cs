namespace GestionClienteHL.Entidades.Consultas;

public class GraficarPedidosHumalabRequest
{
	public int Ordenes { get; set; }
	public int idCliente { get; set; }
	public string numRemision { get; set; } = string.Empty;
	public string nomPrueba { get; set; } = string.Empty;
}
