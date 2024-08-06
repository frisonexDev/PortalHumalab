using CorreoHL.Datos;
using CorreoHL.Entidades.Correo;
using Microsoft.AspNetCore.Mvc;

namespace CorreoHL.Controllers;

[ApiController]
[Route("api/Humalab/CorreoHl")]
public class CorreoController : ControllerBase
{
	private readonly ILogger<CorreoController> _logger;
	private readonly IMapeoDatosCorreo mapeoDatosCorreo;

	public CorreoController(
		ILogger<CorreoController> logger,
		IMapeoDatosCorreo _mapeoDatosCorreo)
	{
		_logger = logger;
		mapeoDatosCorreo = _mapeoDatosCorreo;
	}

	[HttpPost("clientenuevo")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<int>> ClienteNuevo(
		[FromBody] ClienteNuevo request)
	{
		try
		{			
			// Ejecución de la operación de datos
			var response = 400;
			await Task.Factory.StartNew(async () =>
			{
				response = await mapeoDatosCorreo.ClienteNuevo(request);
			});

			return Created(string.Empty, response);
		}
		catch (Exception e)
		{
			return null;
		}
	}

	[HttpPost("pedidoCorreo")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<int>> pedidoCorreo(
			[FromBody] PedidoCorreo request)
	{
		try
		{			
			// Ejecución de la operación de datos
			var response = 400;
			await Task.Factory.StartNew(async () =>
			{
				response = await mapeoDatosCorreo.PedidoCorreo(request);
			});

			return Created(string.Empty, response);
		}
		catch (Exception e)
		{
			return null!;
		}
	}

	[HttpPost("elimPedidoCorreo")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<int>> elimPedidoCorreo(
			[FromBody] ElimPedidoCorreo request)
	{
		try
		{			
			// Ejecución de la operación de datos
			var response = 400;
			await Task.Factory.StartNew(async () =>
			{
				response = await mapeoDatosCorreo.ElimPedidoCorreo(request);
			});

			return Created(string.Empty, response);
		}
		catch (Exception e)
		{
			return null!;
		}
	}

}