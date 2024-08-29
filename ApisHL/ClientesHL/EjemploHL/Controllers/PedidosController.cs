using ClienteHL.Datos;
using ClienteHL.Entidades.Consultas.Pedidos;
using ClienteHL.Entidades.Operaciones;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static ClienteHL.Utils.Constantes;

namespace ClienteHL.Controllers;

[ApiController]
[Route("api/humalab/Pedido")]

public class PedidosController : ControllerBase
{
	private readonly ILogger<PedidosController> _logger;
	private readonly IMapeoDatosPedido mapeoDatosPedido;

	#region constructor del controlador
	public PedidosController(
		ILogger<PedidosController> logger,
		IMapeoDatosPedido _mapeoDatosPedido)
	{
		_logger = logger;
		mapeoDatosPedido = _mapeoDatosPedido;
	}

	#endregion

	[HttpGet("consultarnumeropedido")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<int>> ConsultarNumeroPedido(
		[FromQuery] ConsultarPedido query)
	{
		int NumeroPedido = 0;

		try
		{
			await Task.Factory.StartNew(() =>
			{
				NumeroPedido = mapeoDatosPedido.ObtenerNumeroPedido(query);
			});

			return Ok(NumeroPedido);
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	[HttpGet("listarpedidos")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<List<ListaPedidos>>> ListarPedidos(
		[FromQuery] ConsultarPedido Buscar)
	{
		List<ListaPedidos> lista = new List<ListaPedidos>();

		try
		{
			await Task.Factory.StartNew(() =>
			{
				lista = mapeoDatosPedido.ListaDePedidos(Buscar);
			});

			return Ok(lista);
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	[HttpGet("consultaroperador")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<Operador>> ConsultarOperador(
		[FromQuery] ConsultarPedido query)
	{
		Operador operador = new Operador();

		try
		{
			await Task.Factory.StartNew(() =>
			{
				operador = mapeoDatosPedido.ObtenerOperador(query);
			});

			return Ok(operador);
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	[HttpPost("grabarpedido")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<int>> GrabarPedido(
		[FromBody] GrabarPedidosRequest request)
	{
		var response = 400;

		try
		{
			await Task.Factory.StartNew(() =>
			{
				response = mapeoDatosPedido.Grabar(request);
			});

			return Created(string.Empty, response);
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	[HttpPost("eliminarpedido")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<int>> EliminarPedido(
		[FromBody] ListaPedidos Npedido)
	{
		var response = Transaccion.Error;

		try
		{
			await Task.Factory.StartNew(() =>
			{
				response = mapeoDatosPedido.EliminarPedido(Npedido.IdPedido, Npedido.UsuarioCreacion);
			});

			return Created(string.Empty, response);
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	[HttpGet("clienteCorreo")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<DatosClienteCorreo>> clienteCorreo(
		[FromQuery] int idOrden)
	{
		var datosClienteCorreo = new DatosClienteCorreo();

		try
		{
			await Task.Factory.StartNew(() =>
			{
				datosClienteCorreo = mapeoDatosPedido.datosClienteCorreo(idOrden);
			});

			return datosClienteCorreo;
		}
		catch (Exception ex)
		{
			return null!;
		}
	}

	[HttpGet("clienteCorreoElim")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<DatosClienteCorreo>> clienteCorreoElim(
		[FromQuery] int idPedido)
	{
		try
		{
			var datosClienteCorreo = new DatosClienteCorreo();

			await Task.Factory.StartNew(() =>
			{
				datosClienteCorreo = mapeoDatosPedido.datosClienteCorreoElim(idPedido);
			});

			return datosClienteCorreo;
		}
		catch (Exception ex)
		{
			return null!;
		}
	}
}
