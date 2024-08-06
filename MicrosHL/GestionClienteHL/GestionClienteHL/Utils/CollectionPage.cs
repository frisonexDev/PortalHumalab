namespace GestionClienteHL.Utils;

public class CollectionPage<T> where T : class
{
	private int tamanoPagina = 1;

	public IList<T> Data { get; set; }

	public int TotalRegistros { get; set; }

	public int TotalPaginas
	{
		get
		{
			if (TotalRegistros <= 0)
			{
				return 0;
			}

			return Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(TotalRegistros) / (decimal)tamanoPagina));
		}
	}
}
