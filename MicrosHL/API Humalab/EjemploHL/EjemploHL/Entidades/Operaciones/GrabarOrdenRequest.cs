using Newtonsoft.Json;

namespace ClienteHL.Entidades.Operaciones;

public class GrabarOrdenRequest
{
	[JsonProperty("IdOrden")]
	public int? IdOrden { get; set; }

	[JsonProperty("IdPedido")]
	public int? IdPedido { get; set; }

	[JsonProperty("IdUsuarioGalileo")]
	public int? IdUsuarioGalileo { get; set; }

	[JsonProperty("Identificacion")]
	public string? Identificacion { get; set; }

	[JsonProperty("CodigoBarra")]
	public string? CodigoBarra { get; set; }

	[JsonProperty("Medicamento")]
	public string? Medicamento { get; set; }

	[JsonProperty("Diagnostico")]
	public string? Diagnostico { get; set; }

	[JsonProperty("Observacion")]
	public string? Observacion { get; set; }

	[JsonProperty("Estado")]
	public int? Estado { get; set; }

	[JsonProperty("Pruebas")]
	public List<Pruebas> Pruebas { get; set; } = new List<Pruebas>();

	[JsonProperty("Paciente")]
	public Pacientes Paciente { get; set; }

	[JsonProperty("UsuarioCreacion")]
	public int? UsuarioCreacion { get; set; }

	[JsonProperty("FechaCreacion")]
	public DateTime FechaCreacion { get; set; } = DateTime.Now;

	[JsonProperty("EmpresaId")]
	public int? EmpresaId { get; set; }
}

public class Pruebas
{
	[JsonProperty("IdPrueba")]
	public int? IdPrueba { get; set; }

	[JsonProperty("IdPruebaGalileo")]
	public int? IdPruebaGalileo { get; set; }

	[JsonProperty("IdMuestraGalileo")]
	public int? IdMuestraGalileo { get; set; }

	[JsonProperty("IdOrden")]
	public int? IdOrden { get; set; }

	[JsonProperty("CodigoExamen")]
	public string? CodigoExamen { get; set; }

	[JsonProperty("EsPerfil")]
	public bool? EsPerfil { get; set; }

	[JsonProperty("CodigoBarra")]
	public string? CodigoBarra { get; set; }

	[JsonProperty("Nombre")]
	public string? Nombre { get; set; }

	[JsonProperty("Abreviatura")]
	public string? Abreviatura { get; set; }

	[JsonProperty("Metodologia")]
	public string? Metodologia { get; set; }

	[JsonProperty("NombreMuestra")]
	public string? NombreMuestra { get; set; }

	[JsonProperty("MuestraAlterna")]
	public string? MuestraAlterna { get; set; }

	[JsonProperty("Recipiente")]
	public string? Recipiente { get; set; }

	[JsonProperty("Precio")]
	public float? Precio { get; set; }

	[JsonProperty("EstadoMuestra")]
	public int? EstadoMuestra { get; set; }

	[JsonProperty("EstadoPrueba")]
	public int? EstadoPrueba { get; set; }

	[JsonProperty("UsuarioCreacion")]
	public int? UsuarioCreacion { get; set; }

	[JsonProperty("FechaCreacion")]
	public DateTime FechaCreacion { get; set; } = DateTime.Now;
}

public class Muestras
{
	[JsonProperty("IdMuestra")]
	public int? IdMuestra { get; set; }

	[JsonProperty("CodigoBarra")]
	public string? CodigoBarra { get; set; } 

	[JsonProperty("UsuarioCreacion")]
	public int? UsuarioCreacion { get; set; }
}

public class Pacientes
{

	[JsonProperty("Identificacion")]
	public string? Identificacion { get; set; }

	[JsonProperty("Nombres")]
	public string? Nombres { get; set; }

	[JsonProperty("Apellidos")]
	public string? Apellidos { get; set; }

	[JsonProperty("Genero")]
	public bool? Genero { get; set; }

	[JsonProperty("FechaNacimiento")]
	public DateTime FechaNacimiento { get; set; }

	[JsonProperty("Edad")]
	public string? Edad { get; set; }

	[JsonProperty("Telefono")]
	public string? Telefono { get; set; } 

	[JsonProperty("Email")]
	public string? Email { get; set; } 

	[JsonProperty("UsuarioCreacion")]
	public int? UsuarioCreacion { get; set; }

	[JsonProperty("FechaCreacion")]
	public DateTime FechaCreacion { get; set; } = DateTime.Now;

	[JsonProperty("TipoPaciente")]
	public int? TipoPaciente { get; set; } //verificar

	[JsonProperty("CodLaboratorio")]
	public string? CodLaboratorio { get; set; } //verificar
}