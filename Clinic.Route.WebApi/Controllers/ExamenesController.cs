using Clinic.Route.Application.Features.Examenes.Queries;
using Clinic.Route.Application.Interfaces;
using Clinic.Route.Application.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Clinic.Route.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamenesController : ControllerBase
    {
        private readonly GetExamenesPacienteQuery _siglaRepository;
        private readonly IExamenService _service;
        private readonly IExamenRepository _repository;
        private readonly ILogger<ExamenesController> _logger;


        public ExamenesController(IExamenService service, IExamenRepository repository, GetExamenesPacienteQuery siglaRepository, ILogger<ExamenesController> logger)
        {
            _siglaRepository = siglaRepository;
            _service = service;
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("{codEmp}/{codSed}/{codTCl}/{numOrd}")]
        public async Task<IActionResult> GetExamenes(int codEmp, int codSed, int codTCl, int numOrd)
        {
            if (codEmp <= 0 || codSed <= 0 || codTCl <= 0 || numOrd <= 0)
                return BadRequest("Parámetros inválidos");
            try
            {
                // 🔹 Inicia la suscripción en tiempo real
                await _service.StartRealtimeAsync(codEmp, codSed, codTCl, numOrd);
                var data = await _service.GetExamenesAsync(codEmp, codSed, codTCl, numOrd);

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener exámenes del paciente");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("{NumDni}")]
        public async Task<IActionResult> LoginDni(string NumDni)
        {
            // 1. Validar el parámetro de entrada
            if (string.IsNullOrWhiteSpace(NumDni))
            {
                _logger.LogWarning("Login attempt failed: DNI is null or empty.");
                return BadRequest("El número de DNI no puede ser nulo o vacío.");
            }

            try
            {
                // 2. Llamar al método del repositorio para comprobar el estado de inicio de sesión
                var loginResult = await _repository.LoginPorDni(NumDni);

                // 3. Devolver una respuesta HTTP apropiada según el resultado
                if (loginResult == 1)
                {
                    _logger.LogInformation("Inicie sesión exitosamente para DNI: {NumDni}", NumDni);
                    return Ok(new { message = "Login exitoso. El usuario puede acceder.", status = 1 });
                }
                else if (loginResult == 0)
                {
                    _logger.LogWarning("Error al iniciar sesión DNI: {NumDni}. Exámenes ya completados.", NumDni);
                    return Forbid("El usuario ya ha finalizado sus exámenes y no puede acceder.");
                }
                else
                {
                    // Maneja casos donde el DNI podría no existir o el procedimiento devuelve un valor inesperado.
                    _logger.LogWarning("Error al iniciar sesión DNI: {NumDni}. Usuario no encontrado.", NumDni);
                    return NotFound("El número de DNI no se encontró.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el controlador al intentar logear con DNI: {NumDni}", NumDni);
                return StatusCode(500, "Ocurrió un error interno del servidor.");
            }
        }

        // Endpoint opcional para iniciar la suscripción desde el front (una vez por orden)
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest req)
        {
            if (req is null || req.CodEmp <= 0 || req.CodSed <= 0 || req.CodTCl <= 0 || req.NumOrd <= 0)
                return BadRequest("Parámetros inválidos");

            try
            {
                await _service.StartRealtimeAsync(req.CodEmp, req.CodSed, req.CodTCl, req.NumOrd);
                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error iniciando suscripción");
                return StatusCode(500, "Error interno del servidor");
            }
        }

    }
    public class SubscribeRequest
    {
        public int CodEmp { get; set; }
        public int CodSed { get; set; }
        public int CodTCl { get; set; }
        public int NumOrd { get; set; }
    }
}
