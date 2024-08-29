using Azure.Core;
using GestionClienteHL.Datos;
using GestionClienteHL.Entidades.Consultas;
using GestionClienteHL.Entidades.Operaciones;
using Microsoft.AspNetCore.Mvc;

namespace GestionClienteHL.Controllers;

[ApiController]
[Route("api/Humalab/GestionClienteHl")]
public class GestionClienteController : ControllerBase
{
	
	private readonly ILogger<GestionClienteController> _logger;
	private readonly IMapeoDatosGestionCliente mapeoDatosGestionCliente;

	#region constructor
	public GestionClienteController(
		ILogger<GestionClienteController> logger,
		IMapeoDatosGestionCliente _mapeoDatosEjemplo)
	{
		_logger = logger;
		mapeoDatosGestionCliente = _mapeoDatosEjemplo;
	}

	#endregion

	#region controladores metodos
	[HttpGet("estadosCliente")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<List<EstadosHumalabClienteResponse>>> ConsultarEstaHumaCliente()
	{
		try
		{
			var estadoHumaCliente = new List<EstadosHumalabClienteResponse>();

			await Task.Factory.StartNew(() =>
			{
				estadoHumaCliente = mapeoDatosGestionCliente.ConsultarEstadosHumaCliente();
			});

			return estadoHumaCliente;
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	//Revisar este metodo
	[HttpGet("clienteHumalab")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<List<ClientesHumalabEstados>>> ConsultarClienteHumalab(
		[FromQuery] ConsultarClienteHumaQuery query)
	{
		try
		{
			var clienteHumalab = new List<ClientesHumalabEstados>();

			await Task.Factory.StartNew(() =>
			{
				clienteHumalab = mapeoDatosGestionCliente.ConsultarClienteHuma(query);
			});

			return clienteHumalab;
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	[HttpGet("clientesHumalab")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<List<ClientesHumalabEstados>>> ConsultarClientesHumalab(
		[FromQuery] ConsultarClientesQuery query)
	{
		try
		{
			var clientesHumalab = new List<ClientesHumalabEstados>();

			await Task.Factory.StartNew(() =>
			{
				clientesHumalab = mapeoDatosGestionCliente.ConsultarClientesHumalab(query);
			});

			return clientesHumalab;
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	[HttpPut("modificarClienteHuma")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<GestionClienteEstadoResponse>> gestionClienteEstadoHuma(
		[FromBody] GestionClienteEstadoRequest request)
	{
		try
		{
			var response = new GestionClienteEstadoResponse();

			await Task.Factory.StartNew(() =>
			{
				response = mapeoDatosGestionCliente.GestionClienteEstadoHuma(request);
			});

			return Created(string.Empty, response);
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	[HttpPost("darAltaClienteHumalab")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<DarAltaGrabarClienteHumalabResponse>> grabarClienteHumalab(
		[FromBody] DarAltaGrabarClienteHumalabRequest request)
	{
		try
		{
			var response = new DarAltaGrabarClienteHumalabResponse();

			await Task.Factory.StartNew(() =>
			{
				response = mapeoDatosGestionCliente.GrabarClienteHumalab(request);
			});

			return Created(string.Empty, response);
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	[HttpPut("darAltaModificarHumalab")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<DarAltaGrabarClienteHumalabResponse>> darAltaModificarHumalab(
		[FromBody] DarAltaGrabarClienteHumalabRequest request)
	{
		try
		{
			var response = new DarAltaGrabarClienteHumalabResponse();

			await Task.Factory.StartNew(() =>
			{
				response = mapeoDatosGestionCliente.ModificarHumalabDarAlta(request);
			});

			return Created(string.Empty, response);
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	[HttpGet("pedidosHumalab")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<List<PedidoHumalabResponse>>> pedidosHumalab(
		[FromQuery] int idCliente)
	{
		try
		{
			var pedidosHumalab = new List<PedidoHumalabResponse>();

			await Task.Factory.StartNew(() =>
			{
				pedidosHumalab = mapeoDatosGestionCliente.PedidosHumalab(idCliente);
			});

			return pedidosHumalab;
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	[HttpGet("pedidosOrdenHumalab")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<PedidoOrdenHumalabResponse>> pedidosOrdenHumalab(
		[FromQuery] string numRemision, int idCliente)
	{
		try
		{
			var pedidosOrdenHumalab = new PedidoOrdenHumalabResponse();

			await Task.Factory.StartNew(() =>
			{
				pedidosOrdenHumalab = mapeoDatosGestionCliente.PedidoOrdenHumalab(numRemision, idCliente);
			});

			return pedidosOrdenHumalab;
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	[HttpGet("pedidoEstadosHumalab")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<GraficarPedidosHumalabResponse>> pedidoEstadosHumalab(
		[FromQuery] GraficarPedidosHumalabRequest graficarPedidos)
	{
		try
		{
			var pedidos = new GraficarPedidosHumalabResponse();

			await Task.Factory.StartNew(() =>
			{
				pedidos = mapeoDatosGestionCliente.PedidosEstadosHumalab(graficarPedidos);
			});

			return pedidos;
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	[HttpGet("pedidoTodosHumalab")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<GraficarPedidosHumalabResponse>> pedidoTodosHumalab(
		[FromQuery] int idCliente)
	{
		try
		{
			var pedidos = new GraficarPedidosHumalabResponse();

			await Task.Factory.StartNew(() =>
			{
				pedidos = mapeoDatosGestionCliente.PedidosTodosHumalab(idCliente);
			});

			return pedidos;
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	[HttpGet("muestrasHumalab")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<MuestrasHumalabResponse>> muestrasHumalab(
		[FromQuery] int idCliente)
	{
		try
		{
			var muestrasHumalab = new MuestrasHumalabResponse();

			await Task.Factory.StartNew(() =>
			{
				muestrasHumalab = mapeoDatosGestionCliente.MuestrasHumalab(idCliente);
			});

			return Ok(muestrasHumalab);
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	[HttpGet("observacionCliente")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<string>> observacionCliente(
		[FromQuery] int idUsuario)
	{
		try
		{			
			string observacion = "";

			await Task.Factory.StartNew(() =>
			{
				observacion = mapeoDatosGestionCliente.ConsultarObservacionCliente(idUsuario);
			});

			return observacion;
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	[HttpGet("direccionCliente")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<ClienteHumalabDirecc>> direccionCliente(
		[FromQuery] string ruc, string accion)
	{
		try
		{			
			ClienteHumalabDirecc cliente = new ClienteHumalabDirecc();

			await Task.Factory.StartNew(() =>
			{
				cliente = mapeoDatosGestionCliente.ConsultarDirecCliente(ruc, accion);
			});

			return cliente;
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	#endregion


	#region Respuesta servicios

	//protected ObjectResult ResponseFault(Exception e)
	//{
	//	string text = ApiCodes.CODE_ERROR_API_01;
	//	string text2 = ApiCodes.ERROR_API_01;
	//	RequestException ex = e as RequestException;
	//	if (ex != null)
	//	{
	//		return ResponseBadRequest(ex.Code, ex.Message);
	//	}

	//	if (e is DifareDataException)
	//	{
	//		text = ApiCodes.CODE_ERROR_API_02;
	//		text2 = ApiCodes.ERROR_API_02;
	//	}

	//	string errorMessage = $"{text}|{text2}|{e.Message}";
	//	int statusCode = 500;
	//	return StatusCode(statusCode);
	//}

	//protected BadRequestObjectResult ResponseBadRequest(string code, string description)
	//{
	//	Fault error = LogHandler.SendErrorLog(400, code, description, null, string.Empty);
	//	return BadRequest(error);
	//}

	#endregion
}