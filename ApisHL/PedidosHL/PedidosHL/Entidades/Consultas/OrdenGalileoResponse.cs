using Newtonsoft.Json;
using System.Reflection.Emit;

namespace PedidosHL.Entidades.Consultas;

public class OrdenGalileoResponse: BaseResponse
{
	[JsonProperty("Cliente")]
	public string Cliente { get; set; } = string.Empty;

	[JsonProperty("FechaIngreso")]
	public DateTime FechaIngreso { get; set; }

	[JsonProperty("Comentario")]
	public string Comentario { get; set; } = string.Empty;

	[JsonProperty("codigoExterno")]
	public string codigoExterno { get; set; } = string.Empty;

	[JsonProperty("Identificador")]
	public string Identificador { get; set; } = string.Empty;

	[JsonProperty("NombrePaciente")]
	public string NombrePaciente { get; set; } = string.Empty;

	[JsonProperty("ApellidoPaciente")]
	public string ApellidoPaciente { get; set; } = string.Empty;

	[JsonProperty("FechaNacimiento")]
	public DateTime FechaNacimiento { get; set; }

	[JsonProperty("Genero")]
	public string Genero { get; set; } = string.Empty;

	[JsonProperty("CorreoElectronico")]
	public string CorreoElectronico { get; set; } = string.Empty;

	[JsonProperty("nombreCampoAdicional")]
	public string nombreCampoAdicional { get; set; } = string.Empty;

	[JsonProperty("valor")]
	public string valor { get; set; } = string.Empty;

	[JsonProperty("Nombre")]
	public string Nombre { get; set; } = string.Empty;

	[JsonProperty("Apellido")]
	public string Apellido { get; set; } = string.Empty;

	[JsonProperty("Matricula")]
	public string Matricula { get; set; } = string.Empty;

	[JsonProperty("Telefono")]
	public string Telefono { get; set; } = string.Empty;
	
	[JsonProperty("RucCliente")]
	public string RucCliente { get; set; } = string.Empty;

    [JsonProperty("Email")]
	public string Email { get; set; } = string.Empty;

	[JsonProperty("medicamento")]
	public string medicamento { get; set; } = string.Empty;

	[JsonProperty("diagnostico")]
	public string diagnostico { get; set; } = string.Empty;

	[JsonProperty("CodigoExamen")]
	public string CodigoExamen { get; set; } = string.Empty;

	[JsonProperty("NombreExamenPerfil")]
	public string NombreExamenPerfil { get; set; } = string.Empty;

	public OrdenGalileoResponse()
		: base(true, ApiCodes.CODE_ERROR_API_00, ApiCodes.ERROR_API_00) { }

	internal OrdenGalileoResponse(string codigo, string mensaje)
	: base(false, codigo, mensaje) { }
}