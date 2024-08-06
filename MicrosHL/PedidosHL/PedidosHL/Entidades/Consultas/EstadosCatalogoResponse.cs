using System.ComponentModel.DataAnnotations;

namespace PedidosHL.Entidades.Consultas;

public class EstadosCatalogoResponse
{
	[Display(Name = "IdCatalogoDetalle")]
	public int IdCatalogoDetalle { get; set; }

	[Display(Name = "Nombre")]
	public string Nombre { get; set; } = string.Empty;

	[Display(Name = "Valor")]
	public string Valor { get; set; } = string.Empty;
}
