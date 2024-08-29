using GDifare.Portal.Humalab.Servicio.Modelos.PedidosLaboratorista;

namespace GDifare.Portal.HumaLab.UI.Models.Pedidos;

public class ExportarDatosResponse
{
    public string CodigoBarraOrden { get; set; } = string.Empty;
    public List<ListaMuestras> muestras { get; set; } = new List<ListaMuestras>();
    public string NombresPac { get; set; } = string.Empty;
    public string IdentificacionPac { get; set; } = string.Empty;  
    public string NombreCliente {  get; set; } = string.Empty;
    public string CiudadCliente {  get; set; } = string.Empty;
}

public class ListaMuestras
{
    public string NombreMuestra { get; set; } = string.Empty;
    public string NombreExamen { get; set; } = string.Empty;
    public string CodExamen { get; set; } = string.Empty;
}
