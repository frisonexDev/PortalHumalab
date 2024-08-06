namespace GeneradorHL.Entidades.Consultas;

public class CorreoCliente
{
	public string Usuario { get; set; }
	public string Contrasena { get; set; }
	public string Url { get; set; }
	public int UsuarioCreacion { get; set; }
	public DateTime FechaCreacion { get; set; } = DateTime.Now;
}
