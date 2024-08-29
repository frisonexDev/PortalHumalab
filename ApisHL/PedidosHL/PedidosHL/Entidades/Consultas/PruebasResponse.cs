using Newtonsoft.Json;
using System.Reflection.Emit;

namespace PedidosHL.Entidades.Consultas;

public class PruebasResponse: BaseResponse
{
	[JsonProperty("IdOrden")]
	public int IdOrden { get; set; }

	[JsonProperty("IdPrueba")]
	public int IdPrueba { get; set; }

	[JsonProperty("Codigo")]
	public string Codigo { get; set; } = string.Empty;

	[JsonProperty("PruebaPerfil")]
	public string PruebaPerfil { get; set; } = string.Empty;

	[JsonProperty("PruebaRechazada")]
	public bool PruebaRechazada { get; set; }

	[JsonProperty("ObservacionPrueba")]
	public string ObservacionPrueba { get; set; } = string.Empty;
	public PruebasResponse()
	: base(true, ApiCodes.CODE_ERROR_API_00, ApiCodes.ERROR_API_00) { }

	internal PruebasResponse(string codigo, string mensaje)
		: base(false, codigo, mensaje) { }
}
