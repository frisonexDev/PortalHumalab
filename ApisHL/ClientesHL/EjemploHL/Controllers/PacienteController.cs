using ClienteHL.Datos;
using ClienteHL.Entidades;
using ClienteHL.Entidades.Consultas.Ordenes;
using EjemploHL.Controllers;
using EjemploHL.Datos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClienteHL.Controllers;

[ApiController]
[Route("api/humalab/Paciente")]
public class PacienteController : ControllerBase
{
	private readonly ILogger<PacienteController> _logger;
	private readonly IMapeoDatosPaciente mapeoDatosPaciente;

	#region constructor del controlador
	public PacienteController(
		ILogger<PacienteController> logger,
		IMapeoDatosPaciente _mapeoDatosPaciente)
	{
		_logger = logger;
		mapeoDatosPaciente = _mapeoDatosPaciente;
	}

	#endregion

	[HttpGet("consultarpaciente")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<Paciente>> ConsultarPaciente(
		[FromQuery] ConsultarPaciente query)
	{
		Paciente DatosPaciente = new Paciente();

		try
		{
			await Task.Factory.StartNew(() =>
			{
				DatosPaciente = mapeoDatosPaciente.ObtenerPaciente(query);
			});

			return Ok(DatosPaciente);
		}
		catch(Exception ex)
		{
			return null!;
		}
	}

}
