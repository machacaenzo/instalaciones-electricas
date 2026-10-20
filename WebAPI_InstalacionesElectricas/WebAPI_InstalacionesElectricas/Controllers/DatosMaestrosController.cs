using Microsoft.AspNetCore.Mvc;
using WebAPI_InstalacionesElectricas.Services;

namespace WebAPI_InstalacionesElectricas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatosMaestrosController : ControllerBase
    {
        private readonly RolService _rolService;
        private readonly EstadoSolicitudService _estadoService;

        public DatosMaestrosController(RolService rolService, EstadoSolicitudService estadoService)
        {
            _rolService = rolService;
            _estadoService = estadoService;
        }

        [HttpGet("roles")]
        public async Task<IActionResult> ObtenerRoles()
        {
            try
            {
                return Ok(await _rolService.ObtenerTodosAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al cargar los roles.", detalle = ex.Message });
            }
        }

        [HttpGet("estados")]
        public async Task<IActionResult> ObtenerEstados()
        {
            try
            {
                return Ok(await _estadoService.ObtenerTodosAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al cargar los estados.", detalle = ex.Message });
            }
        }
    }
}