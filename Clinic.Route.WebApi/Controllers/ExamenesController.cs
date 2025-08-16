using Clinic.Route.Application.Features.Examenes.Queries;
using Clinic.Route.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Clinic.Route.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamenesController : ControllerBase
    {
        private readonly GetExamenesPacienteQuery _siglaRepository;
        private readonly ILogger<ExamenesController> _logger;
     

        public ExamenesController(GetExamenesPacienteQuery siglaRepository, ILogger<ExamenesController> logger)
        {
            _siglaRepository = siglaRepository;
            _logger = logger;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetExamenes(
        //[FromQuery] int codEmp,
        //[FromQuery] int codSed,
        //[FromQuery] int codTCl,
        //[FromQuery] int numOrd)
        //{
        [HttpGet("{codEmp}/{codSed}/{codTCl}/{numOrd}")]
        public async Task<IActionResult> GetExamenes(int codEmp, int codSed, int codTCl, int numOrd)
        {
            try
            {
                var examenes = await _siglaRepository.ExecuteAsync(codEmp, codSed, codTCl, numOrd);
                return Ok(examenes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener exámenes del paciente");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
