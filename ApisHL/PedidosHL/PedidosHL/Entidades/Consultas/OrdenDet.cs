namespace PedidosHL.Entidades.Consultas;

public class OrdenDet
{
	public int IdOrden { get; set; }
	public int IdPrueba { get; set; }
	public int Codigo { get; set; }
	public string PruebaPerfil { get; set; } = string.Empty;
	public string CodigoExamen {  get; set; } = string.Empty;
    public bool PruebaRechazada { get; set; }
	public string ObservacionPrueba { get; set; } = string.Empty;
	public string Muestra { get; set; } = string.Empty;
	public string CodigoBarra { get; set; } = string.Empty;
	public int IdMuestra { get; set; }
	public bool Recibido { get; set; }
	public bool Rechazado { get; set; }
	public string ObservacionMuestra { get; set; } = string.Empty;
	public string EstadoMuestra { get; set; } = string.Empty;
	public string EstadoOrden { get; set; } = string.Empty;
	public string FechaCreacion { get; set; } = string.Empty;
	public string CodLis { get; set; } = string.Empty;	
	public string EstadoPrueba { get; set; } = string.Empty;
}
