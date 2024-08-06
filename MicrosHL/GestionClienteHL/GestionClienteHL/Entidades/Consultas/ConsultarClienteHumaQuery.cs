using GestionClienteHL.Utils;
using Newtonsoft.Json;

namespace GestionClienteHL.Entidades.Consultas;

public class ConsultarClienteHumaQuery: PagedViewRequest
{
	[JsonProperty("RucNombre")]
	public string RucNombre { get; set; } = string.Empty;

	[JsonProperty("idEstado")]
	public int idEstado { get; set; }
}
