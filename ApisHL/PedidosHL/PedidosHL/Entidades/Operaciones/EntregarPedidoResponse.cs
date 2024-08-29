using Newtonsoft.Json;
using PedidosHL.Entidades.Consultas;
using System.Reflection.Emit;

namespace PedidosHL.Entidades.Operaciones;

public class EntregarPedidoResponse: BaseResponse
{
	[JsonProperty("Entregado")]
	public bool Entregado { get; set; }
	public EntregarPedidoResponse()
	: base(true, ApiCodes.CODE_ERROR_API_00, ApiCodes.ERROR_API_00) { }

	internal EntregarPedidoResponse(string codigo, string mensaje)
		: base(false, codigo, mensaje) { }
}
