using Newtonsoft.Json;
using PedidosHL.Entidades.Consultas;
using System.Reflection.Emit;

namespace PedidosHL.Entidades.Operaciones;

public class CambiarPruebaResponse: BaseResponse
{
	[JsonProperty("IdMuestra")]
	public int IdMuestra { get; set; }
	[JsonProperty("IdPrueba")]
	public int IdPrueba { get; set; }
	[JsonProperty("RechazaMuestra")]
	public bool RechazaMuestra { get; set; }
	[JsonProperty("ActivaMuestra")]
	public bool ActivaMuestra { get; set; }
	[JsonProperty("EstadoPrueba")]
	public string EstadoPrueba { get; set; } = string.Empty;
	public CambiarPruebaResponse()
	: base(true, ApiCodes.CODE_ERROR_API_00, ApiCodes.ERROR_API_00) { }

	internal CambiarPruebaResponse(string codigo, string mensaje)
		: base(false, codigo, mensaje) { }
}
