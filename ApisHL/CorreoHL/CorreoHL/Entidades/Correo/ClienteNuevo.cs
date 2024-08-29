namespace CorreoHL.Entidades.Correo;

public class ClienteNuevo
{
	public string Usuario { get; set; } = string.Empty;
	public string Contrasena { get; set; } = string.Empty;
	public string Url { get; set; } = string.Empty;
	public int UsuarioCreacion { get; set; }
	public DateTime FechaCreacion { get; set; } = DateTime.Now;
	public string NomUsuario { get; set; } = string.Empty;
}
