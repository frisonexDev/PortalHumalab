namespace PedidosHL.Entidades.Consultas;

public class OrdenLaboratoristaDet
{
	public int IdOrden { get; set; }
	public int IdPedido { get; set; }
	public string CodigoBarra { get; set; } = string.Empty;
	public DateTime FechaCreacion { get; set; }
	public string UsuarioOperador { get; set; } = string.Empty;
	public string EstadoPedido { get; set; } = string.Empty;
	public string ObservacionMuestras { get; set; } = string.Empty;
	public string Resultados { get; set; } = string.Empty;
	public string IdentificacionPac { get; set; } = string.Empty;
	public string NombresPac { get; set; } = string.Empty;
	public string ClienteNombre { get; set; } = string.Empty;	
}
