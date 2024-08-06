using Newtonsoft.Json;

namespace PedidosHL.Entidades.Operaciones;

public class GrabarEjemploRequest
{
	[JsonProperty("idEjemplo")]
	public int IdEjemplo { get; set; }

	[JsonProperty("campoRequerimiento")]
	public string CampoRequerimiento { get; set; } = string.Empty;
}
