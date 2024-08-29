namespace PedidosHL.Entidades.Consultas;

public class VerDetallePedidoOperadorResponse
{
	public VerDetallePedidoOperadorCab Pedido { get; set; }
	public List<VerDetallePedidoOperadorDet> Muestras { get; set; }
}
