using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacturacionHL.Entidades.Consultas;

public class ConsolidarFacturacion
{
	[Column("datosFactura")]
	[JsonProperty("datosFactura")]
	public FacturaRequest? datosFactura { get; set; }

	[Column("clienteRequest")]
	[JsonProperty("clienteRequest")]
	public ClienteRequest? clienteRequest { get; set; }
}
