using Newtonsoft.Json;

namespace GestionClienteHL.Utils;

public class PagedViewRequest
{
	[JsonProperty("offset")]
	public int Offset { get; set; }
	[JsonProperty("limit")]
	public int Limit { get; set; }
}
