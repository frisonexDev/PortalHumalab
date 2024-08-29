namespace PedidosHL.Entidades.Consultas;

public class VerOrdenLaboratoristaResponse
{
	public OrdenCab Orden { get; set; }
	public List<OrdenDet> PruebasMuestras { get; set; }
}
