using ClienteHL.Datos;
using ClienteHL.Entidades.Consultas;
using ClienteHL.Entidades.Consultas.Medico;
using ClienteHL.Entidades.Consultas.Ordenes;
using ClienteHL.Entidades.Consultas.Resultados;
using ClienteHL.Entidades.Operaciones;
using EjemploHL.Controllers;
using EjemploHL.Datos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using static ClienteHL.Utils.Constantes;

namespace ClienteHL.Controllers
{
	[ApiController]
	[Route("api/humalab/Cliente")]
	public class OrdenController : ControllerBase
	{
		private readonly ILogger<OrdenController> _logger;
		private readonly IMapeoDatosOrden mapeoDatosOrden;

		#region constructor del controlador
		public OrdenController(
			ILogger<OrdenController> logger,
			IMapeoDatosOrden _mapeoDatosOrden)
		{
			_logger = logger;
			mapeoDatosOrden = _mapeoDatosOrden;
		}
		#endregion

		[HttpGet("consultarnumeroorden")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<int>> ConsultarNumeroOrden(
			[FromQuery] ConsultarOrden query)
		{
			int NumeroOrden = Transaccion.Default;
			try
			{				
				await Task.Factory.StartNew(() =>
				{
					NumeroOrden = mapeoDatosOrden.ObtenerNumeroOrden(query);
				});

				return Ok(NumeroOrden);
			}
			catch (Exception ex)
			{
				return 404;
			}			
		}

		[HttpGet("consultarorden")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<GrabarOrdenRequest>> ConsultarOrden(
			[FromQuery] int IdOrden)
		{
			GrabarOrdenRequest orden = new GrabarOrdenRequest();

			try
			{
				await Task.Factory.StartNew(() =>
				{
					orden = mapeoDatosOrden.ObtenerOrden(IdOrden);
				});

				return Ok(orden);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("numeroorden")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<int>> NumeroOrden(
			[FromQuery] string CodigoBarra)
		{
			int orden = Transaccion.Default;
			try
			{
				await Task.Factory.StartNew(() =>
				{
					orden = mapeoDatosOrden.GetIdOrden(CodigoBarra);
				});

				return Ok(orden);
			}
			catch (Exception ex)
			{
				return 404;
			}
		}

		[HttpGet("consultarprueba")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<Pruebas>>> ConsultarPruebas(
			[FromQuery] int IdOrden, [FromQuery] int IdUsuario)
		{
			List<Pruebas> prueba = new List<Pruebas>();

			try
			{
				await Task.Factory.StartNew(() =>
				{
					prueba = mapeoDatosOrden.ListarPruebas(IdOrden);
				});

				return Ok(prueba);
			}
			catch(Exception ex)
			{
				return null;
			}
		}

		[HttpGet("consultarobservaciones")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<LogObservaciones>>> ConsultarObservaciones(
			[FromQuery] string CodigoBarra)
		{
			List<LogObservaciones> obs = new List<LogObservaciones>();

			try
			{
				await Task.Factory.StartNew(() =>
				{
					obs = mapeoDatosOrden.Observaciones(CodigoBarra);
				});

				return Ok(obs);
			}
			catch(Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("listarordenes")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<ListarOrden>>> ListarOrdenes(
			[FromQuery] ConsultarOrden query)
		{
			List<ListarOrden> lista = new List<ListarOrden>();

			try
			{				
				await Task.Factory.StartNew(() =>
				{
					lista = mapeoDatosOrden.ListarOrdenes(query);
				});

				return Ok(lista);
			}
			catch(Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("listarmuestras")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<Muestras>>> ListarMuestras(
			[FromQuery] int IdOrden)
		{
			List<Muestras> lista = new List<Muestras>();

			try
			{
				await Task.Factory.StartNew(() =>
				{
					lista = mapeoDatosOrden.ListaMuestra(IdOrden);
				});

				return Ok(lista);
			}
			catch(Exception ex)
			{
				return null!;
			}
		}

		[HttpPost("grabarorden")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<int>> GrabarOrden(
			[FromBody] GrabarOrdenRequest request)
		{
			int response = Transaccion.Default;

			try
			{
				await Task.Factory.StartNew(() =>
				{
					response = mapeoDatosOrden.Grabar(request);
				});

				return Created(string.Empty, response);
			}
			catch(Exception ex)
			{
				return null!;
			}
		}

		[HttpPost("actualizarorden")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<DatosResponse>> ActualizarOrden(
			[FromBody] GrabarOrdenRequest request)
		{
			int response = Transaccion.Default;

			try
			{
				await Task.Factory.StartNew(() =>
				{
					response = mapeoDatosOrden.Actualizar(request);
				});

				return Created(string.Empty, response);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpPost("eliminarorden")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<int>> EliminarOrden(
			[FromBody] GrabarOrdenRequest request)
		{
			int response = Transaccion.Default;

			try
			{
				await Task.Factory.StartNew(() =>
				{
					response = mapeoDatosOrden.Eliminar(request);
				});

				return Created(string.Empty, response);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpPost("eliminarpruebas")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<DatosResponse>> EliminarPruebas(
			[FromBody] Pruebas request)
		{
			int response = Transaccion.Default;

			try
			{
				await Task.Factory.StartNew(() =>
				{
					response = mapeoDatosOrden.EliminarPruebas(request);
				});

				return Created(string.Empty, response);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpPost("guardarresultados")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<int>> GuardarResultados(
			[FromQuery] string CodigoBarra, [FromBody] string Resultados, 
			[FromQuery] int UsuarioCreacion)
		{
			int response = Transaccion.Error;

			try
			{
				// Ejecución de la operación de datos
				await Task.Factory.StartNew(() =>
				{
					response = mapeoDatosOrden.GrabarResultados(CodigoBarra, Resultados, UsuarioCreacion);
				});

				return Created(string.Empty, response);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("obtenerresultados")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<Muestras>>> ObtenerResultados(
			[FromQuery] string CodigoBarra)
		{
			string pdf = string.Empty;

			try
			{
				await Task.Factory.StartNew(() =>
				{
					pdf = mapeoDatosOrden.ConsultarResultados(CodigoBarra);
				});

				return Ok(pdf);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("nombreEstadoOrden")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<string>> nombreEstadoOrden(
			[FromQuery] string idOrdenEstado)
		{			
			string nombre = string.Empty;

			try
			{
				await Task.Factory.StartNew(() =>
				{
					nombre = mapeoDatosOrden.nombreEstadoOrden(idOrdenEstado);
				});

				return Ok(nombre);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpPost("actualizaEstadoOrdenPdf")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<string>> ActuEstadoOrdenPdf(
			[FromHeader] string REFERENCE_ID,
			[FromHeader] string CONSUMER,
			[FromBody] ActualizaPdfOrden actualizaPdfOrden)
		{
			string response = string.Empty;

			try
			{
				await Task.Factory.StartNew(() =>
				{
					response = mapeoDatosOrden.ActuOrdenPdfResult(actualizaPdfOrden);
				});

				return response;
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("idClienteOrden")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<int>> idClienteOrden(
			[FromQuery] int idGalileo)
		{
			int id = 0;

			try
			{
				string accion = "L";
				await Task.Factory.StartNew(() =>
				{
					id = mapeoDatosOrden.CLienteIdOrden(idGalileo, accion);
				});

				return Ok(id);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}


		[HttpGet("idClientePedido")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<int>> idClientePedido(
			[FromQuery] int idGalileo)
		{
			int id = 0;

			try
			{				
				await Task.Factory.StartNew(() =>
				{
					id = mapeoDatosOrden.CLienteIdPedido(idGalileo);
				});

				return Ok(id);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}


		[HttpPost("actualizaPdfordenFinal")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<string>> ActualizaPdfordenFinal(
			[FromQuery] string codOrdenHuma, string codOrdenLis,
			int idLab, int idOrdenEstado, [FromBody] string pdfBase64)
		{
			string resultado = "";

			try
			{
				await Task.Factory.StartNew(() =>
				{
					resultado = mapeoDatosOrden.ActualizaResultadoFinal(codOrdenHuma, codOrdenLis,
																		idLab, pdfBase64, idOrdenEstado);
				});

				return Ok(resultado);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("resultadoPdfinal")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<string>> ResultadoPdfinal(
			[FromQuery] string codBarra)
		{
			string pdf = "";

			try
			{
				await Task.Factory.StartNew(() =>
				{
					pdf = mapeoDatosOrden.ConsultarPdfFinal(codBarra);
				});

				return Ok(pdf);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpPost("resultadosLab")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<string>> ResultadosLab(
			[FromQuery] ResultadosRequest resultadosRequest,
			[FromBody] string Informe)
		{
			string resultado = "";

			try
			{
				await Task.Factory.StartNew(() =>
				{
					resultado = mapeoDatosOrden.InsertarResultadosLab(resultadosRequest, Informe);
				});

				return Ok(resultado);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("listarResultadosLab")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<ResultadosMedico>>> ListasResultMedic(
			[FromQuery] ConsultarResultados consultar)
		{
			List<ResultadosMedico> lista = new List<ResultadosMedico>();

			try
			{
				await Task.Factory.StartNew(() =>
				{
					lista = mapeoDatosOrden.ResultadosLabMedico(consultar);
				});

				return Ok(lista);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("resultadoPdfinalMedico")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<string>> ResultadoPdfinalMedico(
			[FromQuery] string codBarra, int idResult)
		{
			string resultado = "";

			try
			{
				await Task.Factory.StartNew(() =>
				{
					resultado = mapeoDatosOrden.ConsultarPdfFinalResult(codBarra, idResult);
				});

				return Ok(resultado);
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		[HttpGet("ResultadoPdf")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<string>> resultPdfFinalMed(
			[FromQuery] int idLab, string codOrden)
		{
			string resultPdf = "";
			try
			{
				await Task.Factory.StartNew(() =>
				{
					resultPdf = mapeoDatosOrden.ConsultResultPdfNew(idLab, codOrden);
				});

				return Ok(resultPdf);
			}
			catch (Exception ex)
			{
				return null!;
			}
		}

		[HttpGet("ResultadoPaciente")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<ResultPacienteResponse>>> PacienteResultado(
			[FromQuery] string Identificacion, int idLab)
		{
			var response = await mapeoDatosOrden.ConsultarResultadosPaciente(Identificacion, idLab);
			return Ok(response);
		}
	}
}
