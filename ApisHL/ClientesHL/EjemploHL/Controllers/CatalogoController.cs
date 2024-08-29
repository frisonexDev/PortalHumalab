using ClienteHL.Entidades.Consultas.CatalogoMaestro;
using EjemploHL.Datos;
using EjemploHL.Entidades.Consultas.CatalogoMaestro;
using Microsoft.AspNetCore.Mvc;

namespace EjemploHL.Controllers
{
    [ApiController]
	[Route("api/humalab/Cliente")]
	public class CatalogoController : ControllerBase
	{
		private readonly ILogger<CatalogoController> _logger;
		private readonly IMapeoDatosCatalogoMaestro mapeoDatosCatalogo;

		#region constructor del controlador
		public CatalogoController(
			ILogger<CatalogoController> logger,
			IMapeoDatosCatalogoMaestro _mapeoDatosCatalogo)
		{
			_logger = logger;
			mapeoDatosCatalogo = _mapeoDatosCatalogo;
		}

		#endregion

		#region Consultas controlador
		[HttpGet("listarcatalogodetalle")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<CatalogoDetalle>>> ListarCatalogoDetalle(
			[FromQuery] CatalogoRequest query)
		{
			try
			{
				List<CatalogoDetalle> lista = new List<CatalogoDetalle>();
				await Task.Factory.StartNew(() =>
				{
					lista = mapeoDatosCatalogo.ListarCatalogoDetalle(query);
				});

				return Ok(lista);
			}
			catch (Exception ex)
			{
				return null!;
			}			
		}

		[HttpGet("listarestados")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<CatalogoDetalle>>> ListarEstados(
			[FromQuery] string NombreEstado)
		{
			try
			{
				List<CatalogoDetalle> lista = new List<CatalogoDetalle>();
				await Task.Factory.StartNew(() =>
				{
					lista = mapeoDatosCatalogo.ListarEstados(NombreEstado);
				});

				return Ok(lista);
			}
			catch (Exception ex)
			{
				return null!;
			}
			
		}

		[HttpGet("listarTiposClientes")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<CatalogoTiposClientes>>> ListarTiposClientes()
		{
			try
			{
				List<CatalogoTiposClientes> lista = new List<CatalogoTiposClientes>();
				await Task.Factory.StartNew(() =>
				{
					lista = mapeoDatosCatalogo.ListarTiposClientes();
				});

				return Ok(lista);
			}
			catch (Exception ex)
			{
				return null!;
			}			
		}

		#endregion
	}
}