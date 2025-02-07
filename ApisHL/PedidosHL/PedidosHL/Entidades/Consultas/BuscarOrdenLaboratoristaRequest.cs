using Newtonsoft.Json;
using System.ComponentModel;

namespace PedidosHL.Entidades.Consultas;

public class BuscarOrdenLaboratoristaRequest
{
	[JsonProperty("FechaDesde")]
	public DateTime FechaDesde { get; set; }

	[JsonProperty("FechaHasta")]
	public DateTime FechaHasta { get; set; }

	[JsonProperty("Operador")]
	public string? Operador { get; set; }

	[DefaultValue(true)]
	[JsonProperty("BuscarPorRuc")]
	public bool? BuscarPorRuc { get; set; }

	[JsonProperty("Cliente")]
	public string? Cliente { get; set; }

	[JsonProperty("Estado")]
	public string? Estado { get; set; }

	[JsonProperty("IdPedido")]
	public int? IdPedido { get; set; }

	[JsonProperty("IdOrden")]
	public int? IdOrden { get; set; }

    [JsonProperty("IdAsesor")]
    public int? IdAsesor { get; set; }
}
