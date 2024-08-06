using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdministracionHL.Entidades.Consultas;

public class IdentificarRol
{
    [Column("Activo")]
    [JsonProperty("Activo")]
    public bool Activo { get; set; }

    [Column("Inactivo")]
    [JsonProperty("Inactivo")]
    public bool Inactivo { get; set; }
}
