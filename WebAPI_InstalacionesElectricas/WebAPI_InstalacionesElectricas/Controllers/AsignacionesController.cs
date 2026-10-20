using Microsoft.AspNetCore.Mvc;
using WebAPI_InstalacionesElectricas.Models;
using WebAPI_InstalacionesElectricas.Services;

namespace WebAPI_InstalacionesElectricas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsignacionesController : ControllerBase
    {
        private readonly AsignacionService _asignacionService;

        public AsignacionesController(AsignacionService asignacionService)
        {
            _asignacionService = asignacionService;
        }

        // POST: api/Asignaciones 
        [HttpPost]
        public async Task<ActionResult> Asignar(Asignacion asignacion)
        {
            try
            {
                await _asignacionService.AsignarTecnicoAsync(asignacion);

                return Ok(new { mensaje = "Técnico asignado correctamente. La solicitud pasó a estado 'En Ejecución'." });
            }
            catch (Exception ex)
            {
              
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // GET: api/Asignaciones 
        [HttpGet]
        public async Task<ActionResult> ObtenerTodas()
        {
            try
            {
                var lista = await _asignacionService.ObtenerTodasAsync();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el historial de asignaciones.", detalle = ex.Message });
            }
        }
    }
}