using Newtonsoft.Json;
using PedidosHL.Entidades.Consultas;

namespace PedidosHL.Entidades.Operaciones;

public class RecogerPedidoResponse: BaseResponse
{
	[JsonProperty("IdPedido")]
	public int IdPedido { get; set; }
	public RecogerPedidoResponse()
			: base(true, ApiCodes.CODE_ERROR_API_00, ApiCodes.ERROR_API_00) { }

	internal RecogerPedidoResponse(string codigo, string mensaje)
		: base(false, codigo, mensaje) { }
}
