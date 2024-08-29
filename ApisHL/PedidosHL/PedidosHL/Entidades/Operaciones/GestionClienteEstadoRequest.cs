using Newtonsoft.Json;

namespace PedidosHL.Entidades.Operaciones;

public class GestionClienteEstadoRequest
{
	[JsonProperty("idCliente")]
	public int idCliente { get; set; }

	[JsonProperty("Cliente")]
	public string Cliente { get; set; } = string.Empty;

	[JsonProperty("FechaVigencia")]
	public string FechaVigencia { get; set; } = string.Empty;

	[JsonProperty("IdEstado")]
	public int IdEstado { get; set; }

	[JsonProperty("Observacion")]
	public string Observacion { get; set; } = string.Empty;

	[JsonProperty("UsuarioModificacion")]
	public int UsuarioModificacion { get; set; }
}
