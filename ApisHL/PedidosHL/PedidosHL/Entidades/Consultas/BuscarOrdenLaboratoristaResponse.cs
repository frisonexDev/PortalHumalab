namespace PedidosHL.Entidades.Consultas;

public class BuscarOrdenLaboratoristaResponse
{
	public OrdenLaboratoristaCab ResumenMuestra { get; set; }
	public List<OrdenLaboratoristaDet> Ordenes { get; set; }
}
