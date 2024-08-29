using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacturacionHL.Entidades.Consultas;

public class FacturaRequest : ClienteRequest
{
	[Column("numeroFactura")]
	[JsonProperty("numeroFactura")]
	public string? numeroFactura { get; set; }

	[Column("identificacionUsuario")]
	[JsonProperty("identificacionUsuario")]
	public string? identificacionUsuario { get; set; }

	[Column("totalFactura")]
	[JsonProperty("totalFactura")]
	public string? totalFactura { get; set; }

	[Column("totalMuestra")]
	[JsonProperty("totalMuestra")]
	public string? totalMuestra { get; set; }

	[Column("estado")]
	[JsonProperty("estado")]
	public string? estado { get; set; }

	[Column("usuarioCreacion")]
	[JsonProperty("usuarioCreacion")]
	public string? usuarioCreacion { get; set; }

	[Column("Ruc")]
	[JsonProperty("Ruc")]
	public string? Ruc { get; set; }


	[Column("CodClienteCta")]
	[JsonProperty("CodClienteCta")]
	public string? CodClienteCta { get; set; }

	[Column("cliente")]
	[JsonProperty("cliente")]
	public string? cliente { get; set; }

	[Column("fecha")]
	[JsonProperty("fecha")]
	public string? fecha { get; set; }

	[Column("estadoFactura")]
	[JsonProperty("estadoFactura")]
	public string? estadoFactura { get; set; }

	[Column("idOrden")]
	[JsonProperty("idOrden")]
	public List<string>? idOrden { get; set; }

	[Column("idOrdenFinal")]
	[JsonProperty("idOrdenFinal")]
	public string? idOrdenFinal { get; set; }
}
