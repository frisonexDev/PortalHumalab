using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacturacionHL.Entidades.Consultas;

public class ClienteResponse
{

	//[Column("IdCliente")]
	//[JsonProperty("IdCliente")]
	//public string? IdCliente { get; set; }

	//[Column("IdUsuario")]
	//[JsonProperty("IdUsuario")]
	//public int IdUsuario { get; set; }

	[Column("NombreCliente")]
	[JsonProperty("NombreCliente")]
	public string? NombreCliente { get; set; }

	[Column("IdOperadorLogistico")]
	[JsonProperty("IdOperadorLogistico")]
	public string? IdOperadorLogistico { get; set; }

	[Column("NombreOperadorLogistico")]
	[JsonProperty("NombreOperadorLogistico")]
	public string? NombreOperadorLogistico { get; set; }

	[Column("identificacion")]
	[JsonProperty("identificacion")]
	public string? identificacion { get; set; }

	//[Column("IdGalileo")]
	//[JsonProperty("IdGalileo")]
	//public int IdGalileo { get; set; }

	//[Column("Usuario")]
	//[JsonProperty("Usuario")]
	//public string? Usuario { get; set; }



	//[Column("identificacionUsuario")]
	//[JsonProperty("identificacionUsuario")]
	//public string? identificacionUsuario { get; set; }


	//[Column("Estado")]
	//[JsonProperty("Estado")]
	//public string? Estado { get; set; }

	[Column("CodClienteCta")]
	[JsonProperty("CodClienteCta")]
	public string? CodClienteCta { get; set; }

	//[Column("Empresa")]
	//[JsonProperty("Empresa")]
	//public string? Empresa { get; set; }
}
