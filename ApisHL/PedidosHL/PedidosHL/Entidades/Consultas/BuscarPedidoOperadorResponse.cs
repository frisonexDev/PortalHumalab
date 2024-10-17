using Newtonsoft.Json;
using PedidosHL.Utils;
using System.Reflection.Emit;

namespace PedidosHL.Entidades.Consultas;

public class BuscarPedidoOperadorResponse: BaseResponse
{
	[JsonProperty("idPedido")]
	public int IdPedido { get; set; }

	[JsonProperty("idCliente")]
	public int IdCliente { get; set; }

	[JsonProperty("Cliente")]
	public string Cliente { get; set; } = string.Empty;
    
	[JsonProperty("NomLaboratorio")]
    public string NomLaboratorio { get; set; } = string.Empty;

    [JsonProperty("numeroRemision")]
	public string NumeroRemision { get; set; } = string.Empty;

	[JsonProperty("fechaCreacion")]
	public DateTime? FechaCreacion { get; set; }

	[JsonProperty("totalOrdenes")]
	public int TotalOrdenes { get; set; }

	[JsonProperty("totalMuestras")]
	public int TotalMuestras { get; set; }

	[JsonProperty("totalRetiradas")]
	public int TotalRetiradas { get; set; }

	[JsonProperty("fechaRetiro")]
	public DateTime? FechaRetiro { get; set; }

	[JsonProperty("estadoPedido")]
	public string EstadoPedido { get; set; } = string.Empty;

	[JsonProperty("Paciente")]
	public string Paciente { get; set; } = string.Empty;
	public BuscarPedidoOperadorResponse()
	: base(true, ApiCodes.CODE_ERROR_API_00, ApiCodes.ERROR_API_00) { }

	internal BuscarPedidoOperadorResponse(string codigo, string mensaje)
	: base(false, codigo, mensaje) { }
}
public class ApiCodes
{
	public const string CODE_ERROR_API_00 = "00";
	public const string ERROR_API_00 = "Error genérico";
}

public class BaseResponse
{
	public BaseResponse(bool isSuccess, string code, string message)
	{
        procesoExitoso = isSuccess;
        codigoRespuesta = code;
        mensajeRespuesta = message;
	}

	public bool procesoExitoso { get; set; }
	public string codigoRespuesta { get; set; }
	public string mensajeRespuesta { get; set; }
}