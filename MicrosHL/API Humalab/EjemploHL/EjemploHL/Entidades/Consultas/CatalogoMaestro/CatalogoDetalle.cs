namespace EjemploHL.Entidades.Consultas.CatalogoMaestro;

public class CatalogoDetalle
{
    public int IdCatalogoMaestro { get; set; }
    public int IdCatalogoDetalle { get; set; }
    public int Relacion { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public string Orden { get; set; } = string.Empty;
    public bool Editable { get; set; }
    public bool Eliminar { get; set; }
}
