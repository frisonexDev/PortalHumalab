using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdministracionHL.Entidades.Consultas;

public class ClienteHumalab
{
    [Column("estadocliente")]
    [JsonProperty("estadocliente")]
    public string? estadocliente { get; set; }

    [Column("fechasuspension")]
    [JsonProperty("fechasuspension")]
    public string? fechasuspension { get; set; }

    [Column("fecharegistro")]
    [JsonProperty("fecharegistro")]
    public string? fecharegistro { get; set; }

    [Column("ruc")]
    [JsonProperty("ruc")]
    public string? ruc { get; set; }
}
