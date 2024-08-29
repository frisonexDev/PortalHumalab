using GestionClienteHL.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionClienteHL.Entidades.Consultas;

public class ClientesHumalabEstados
{
	[Column("IdCliente")]
	[JsonProperty("IdCliente")]
	public int IdCliente { get; set; }

	[Column("Cliente")]
	[JsonProperty("Cliente")]
	public string Cliente { get; set; } = string.Empty;

	[Column("Ruc")]
	[JsonProperty("Ruc")]
	public string Ruc { get; set; } = string.Empty;

	[Column("Usuario")]
	[JsonProperty("Usuario")]
	public string Usuario { get; set; } = string.Empty;

	[Column("FechaRegistro")]
	[JsonProperty("FechaRegistro")]
	public string FechaRegistro { get; set; } = string.Empty;

	[Column("OperadorLogistico")]
	[JsonProperty("OperadorLogistico")]
	public string OperadorLogistico { get; set; } = string.Empty;

	[Column("EstadoCodigo")]
	[JsonProperty("EstadoCodigo")]
	public int EstadoCodigo { get; set; }

	[Column("Estado")]
	[JsonProperty("Estado")]
	public string Estado { get; set; } = string.Empty;

	[Column("FechaTemporal")]
	[JsonProperty("FechaTemporal")]
	public string FechaTemporal { get; set; } = string.Empty;
}
