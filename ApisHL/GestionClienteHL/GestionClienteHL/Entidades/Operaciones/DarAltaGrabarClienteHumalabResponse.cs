using Newtonsoft.Json;
using System.Reflection.Emit;

namespace GestionClienteHL.Entidades.Operaciones;

public class DarAltaGrabarClienteHumalabResponse: BaseResponse
{

	[JsonProperty("procesoExitoso")]
	protected bool ProcesoExitoso { get; set; }

	[JsonProperty("codigoRespuesta")]
	public string CodigoRespuesta { get; set; }

	[JsonProperty("mensajeRespuesta")]
	public string MensajeRespuesta { get; set; }

	internal DarAltaGrabarClienteHumalabResponse()
	: base(true, ApiCodes.CODE_ERROR_API_00, ApiCodes.ERROR_API_00) { }

	internal DarAltaGrabarClienteHumalabResponse(string codigo, string mensaje)
		: base(false, codigo, mensaje) { }
}

public class ApiCodes
{
	public const string CODE_ERROR_API_00 = "00";
	public const string ERROR_API_00 = "Error genérico";

	public const string CODE_ERROR_API_01 = "01";
	public const string ERROR_API_01 = "";

	public const string CODE_ERROR_API_02 = "";
	public const string ERROR_API_02 = "";
}

public class BaseResponse
{
	public BaseResponse(bool isSuccess, string code, string message)
	{
		IsSuccess = isSuccess;
		Code = code;
		Message = message;
	}

	public bool IsSuccess { get; set; }
	public string Code { get; set; }
	public string Message { get; set; }
}