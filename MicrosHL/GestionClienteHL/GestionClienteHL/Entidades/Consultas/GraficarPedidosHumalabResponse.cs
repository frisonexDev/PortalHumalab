using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionClienteHL.Entidades.Consultas;

public class GraficarPedidosHumalabResponse
{
	[Column("totalPendientes")]
	[JsonProperty("totalPendientes")]
	public int totalPendientes { get; set; }

	[Column("totalEnviadas")]
	[JsonProperty("totalEnviadas")]
	public int totalEnviadas { get; set; }

	[Column("totalProcesadas")]
	[JsonProperty("totalProcesadas")]
	public int totalProcesadas { get; set; }
}
