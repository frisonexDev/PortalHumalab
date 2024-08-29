using Newtonsoft.Json;
using PedidosHL.Entidades.Consultas;
using System.Reflection.Emit;

namespace PedidosHL.Entidades.Operaciones;

public class RecogerMuestraResponse: BaseResponse
{
	[JsonProperty("IdMuestra")]
	public int IdMuestra { get; set; }
	[JsonProperty("EstadoMuestra")]
	public string EstadoMuestra { get; set; } = string.Empty;
	[JsonProperty("Observacion")]
	public string Observacion { get; set; } = string.Empty;
	public RecogerMuestraResponse()
	: base(true, ApiCodes.CODE_ERROR_API_00, ApiCodes.ERROR_API_00) { }

	internal RecogerMuestraResponse(string codigo, string mensaje)
		: base(false, codigo, mensaje) { }
}
