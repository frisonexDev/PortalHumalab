using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PedidosHL.Entidades.Consultas;

public class BuscarPedidoOperadorRequest
{
	[Required]
	[JsonProperty("FechaDesde")]
	public DateTime? FechaDesde { get; set; }
	[Required]
	[JsonProperty("FechaHasta")]
	public DateTime? FechaHasta { get; set; }

	[JsonProperty("Cliente")]
	public string? Cliente { get; set; }

	[Required]
	[DefaultValue(true)]
	[JsonProperty("BuscarPorIdentificacion")]
	public bool? BuscarPorIdentificacion { get; set; }

	[JsonProperty("IdOperadorLogistico")]
	public int? IdOperadorLogistico { get; set; }

	[JsonProperty("Estado")]
	public string? Estado { get; set; }
}
