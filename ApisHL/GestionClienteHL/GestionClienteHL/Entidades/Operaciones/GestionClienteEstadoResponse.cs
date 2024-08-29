using Newtonsoft.Json;

namespace GestionClienteHL.Entidades.Operaciones;

public class GestionClienteEstadoResponse: BaseResponse
{
	[JsonProperty("procesoExitoso")]
	protected bool ProcesoExitoso { get; set; }
	[JsonProperty("codigoRespuesta")]
	public string CodigoRespuesta { get; set; }
	[JsonProperty("mensajeRespuesta")]

	public string MensajeRespuesta { get; set; }
	internal GestionClienteEstadoResponse()
			: base(true, ApiCodes.CODE_ERROR_API_00, ApiCodes.ERROR_API_00) { }

	internal GestionClienteEstadoResponse(string codigo, string mensaje)
		: base(false, codigo, mensaje) { }
}
