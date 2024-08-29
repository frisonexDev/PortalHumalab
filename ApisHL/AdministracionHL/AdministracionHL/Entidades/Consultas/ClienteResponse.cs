using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdministracionHL.Entidades.Consultas;

public class ClienteResponse
{
    [Column("idUsuario")]
    [JsonProperty("idUsuario")]
    public int? idUsuario { get; set; }

    [Column("idGalileo")]
    [JsonProperty("idGalileo")]
    public int? idGalileo { get; set; }

    [Column("Usuario")]
    [JsonProperty("Usuario")]
    public string? Usuario { get; set; }

    [Column("IdRol")]
    [JsonProperty("IdRol")]
    public int? IdRol { get; set; }

    [Column("Estado")]
    [JsonProperty("Estado")]
    public string? Estado { get; set; }

    [Column("Identificacion")]
    [JsonProperty("Identificacion")]
    public string? Identificacion { get; set; }

}
