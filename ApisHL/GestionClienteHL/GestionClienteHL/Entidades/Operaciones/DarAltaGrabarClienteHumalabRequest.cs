using Newtonsoft.Json;

namespace GestionClienteHL.Entidades.Operaciones;

public class DarAltaGrabarClienteHumalabRequest
{
	[JsonProperty("idGalileo")]
	public int? idGalileo { get; set; }

	[JsonProperty("Usuario")]
	public string Usuario { get; set; } = string.Empty;

	[JsonProperty("Identificacion")]
	public string Identificacion { get; set; } = string.Empty;

	[JsonProperty("IdRol")]
	public int? IdRol { get; set; }

	[JsonProperty("Email")]
	public string Email { get; set; } = string.Empty;

	[JsonProperty("UsuarioCreacion")]
	public int? UsuarioCreacion { get; set; }

	[JsonProperty("idAsesorFlex")]
	public string idAsesorFlex { get; set; } = string.Empty;

	[JsonProperty("nombreAsesor")]
	public string nombreAsesor { get; set; } = string.Empty;

	[JsonProperty("nombreGalileo")]
	public string nombreGalileo { get; set; } = string.Empty;

	[JsonProperty("codigoCliente")]
	public string codigoCliente { get; set; } = string.Empty;

	[JsonProperty("DireccionCliente")]
	public string DireccionCliente { get; set; } = string.Empty;

	[JsonProperty("ProvinciaCliente")]
	public string ProvinciaCliente { get; set; } = string.Empty;

	[JsonProperty("CiudadCliente")]
	public string CiudadCliente { get; set; } = string.Empty;

	[JsonProperty("LatitudCliente")]
	public string LatitudCliente { get; set; } = string.Empty;

	[JsonProperty("LongitudCliente")]
	public string LongitudCliente { get; set; } = string.Empty;

	[JsonProperty("IdAsesorLis")]
	public string IdAsesorLis { get; set; } = string.Empty;

	[JsonProperty("telefono")]
	public string Telefono { get; set; } = string.Empty;

	[JsonProperty("LabComercial")]
	public string LabComercial {  get; set; } = string.Empty;
}
