namespace PedidosHL.Entidades.Consultas;

public class OrdenGalileo
{
}

public class PacienteCampoAdicional
{
	public string nombreCampo { get; set; } = string.Empty;
	public string valor { get; set; } = string.Empty;
}

public class Paciente
{
	public string Identificador { get; set; } = string.Empty;
	public string NombrePaciente { get; set; } = string.Empty;
	public string ApellidoPaciente { get; set; } = string.Empty;
	public DateTime FechaNacimiento { get; set; }
	public string Genero { get; set; } = string.Empty;
	public string CorreoElectronico { get; set; } = string.Empty;
	public List<PacienteCampoAdicional> CamposAdicional { get; set; }
}

public class Medico
{
	public string Nombre { get; set; } = string.Empty;
	public string Apellido { get; set; } = string.Empty;
	public string Matricula { get; set; } = string.Empty;
	public string Telefono { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
}

public class DetalleOrden
{
	public string NombreExamenPerfil { get; set; } = string.Empty;
	public string CodigoExamen { get; set; } = string.Empty;
}

public class InformacionOrden
{
	public string nombreCampo { get; set; } = string.Empty;
	public string valor { get; set; } = string.Empty;
}

public class TomaMuestra
{
	public bool tomaRemota { get; set; }
	public string direccionToma { get; set; } = string.Empty;
	public bool activo { get; set; }
}

public class InfoOrden
{
	public int idLaboratorio { get; set; }
	public int idSede { get; set; }
	public string Laboratorio { get; set; } = string.Empty;
	public string Cliente { get; set; } = string.Empty;
	public string CieDiagnostico { get; set; } = string.Empty;
	public string IdSistemaExterno { get; set; } = string.Empty;
	public Paciente Paciente { get; set; }
	public Medico Medico { get; set; }
	public string prioridadOrden { get; set; } = string.Empty;
	public DateTime fechaIngreso { get; set; }
	public string comentario { get; set; } = string.Empty;
	public string codigoExterno { get; set; } = string.Empty;
	public string usuario { get; set; } = string.Empty;
	public List<DetalleOrden> detalleOrden { get; set; }
	public List<InformacionOrden> informacionOrden { get; set; }
	public TomaMuestra tomaMuestra { get; set; }
	public bool activo { get; set; }
	public string RucCliente { get; set; } = string.Empty;
}