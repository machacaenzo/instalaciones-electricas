using Microsoft.AspNetCore.Mvc;
using WebAPI_InstalacionesElectricas.Models;
using WebAPI_InstalacionesElectricas.Services;

namespace WebAPI_InstalacionesElectricas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudesController : ControllerBase
    {
        private readonly SolicitudService _solicitudService;

        public SolicitudesController(SolicitudService solicitudService)
        {
            _solicitudService = solicitudService;
        }

        // GET: api/Solicitudes 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Solicitud>>> ObtenerTodas()
        {
            try
            {
                return Ok(await _solicitudService.ObtenerTodasAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno al recuperar las solicitudes.", detalle = ex.Message });
            }
        }

        // GET: api/Solicitudes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Solicitud>> ObtenerPorId(int id)
        {
            try
            {
                var solicitud = await _solicitudService.ObtenerPorIdAsync(id);
                if (solicitud == null)
                    return NotFound(new { mensaje = "Solicitud no encontrada." });

                return Ok(solicitud);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al buscar la solicitud.", detalle = ex.Message });
            }
        }

        // GET: api/Solicitudes/cliente/5 
        [HttpGet("cliente/{idCliente}")]
        public async Task<ActionResult<IEnumerable<Solicitud>>> ObtenerPorCliente(int idCliente)
        {
            try
            {
                var lista = await _solicitudService.ObtenerPorClienteIdAsync(idCliente);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el historial del cliente.", detalle = ex.Message });
            }
        }

        // GET: api/Solicitudes/instalador/5 
        [HttpGet("instalador/{idInstalador}")]
        public async Task<ActionResult<IEnumerable<Solicitud>>> ObtenerPorInstalador(int idInstalador)
        {
            try
            {
                var lista = await _solicitudService.ObtenerPorInstaladorIdAsync(idInstalador);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener la agenda del técnico.", detalle = ex.Message });
            }
        }

        // POST: api/Solicitudes
        [HttpPost]
        public async Task<ActionResult<Solicitud>> Crear(Solicitud solicitud)
        {
            try
            {
                await _solicitudService.CrearAsync(solicitud);

                return CreatedAtAction(nameof(ObtenerPorId), new { id = solicitud.Id }, solicitud);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "No se pudo crear la solicitud.", detalle = ex.Message });
            }
        }

        // PUT: api/Solicitudes/5/estado
        [HttpPut("{id}/estado")]
        public async Task<IActionResult> ActualizarEstado(int id, [FromBody] int nuevoEstadoId)
        {
            try
            {
                
                var solicitud = await _solicitudService.ObtenerPorIdAsync(id);
                if (solicitud == null) return NotFound(new { mensaje = "Solicitud no encontrada." });

                await _solicitudService.ActualizarEstadoAsync(id, nuevoEstadoId);
                return Ok(new { mensaje = "Estado actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al cambiar el estado.", detalle = ex.Message });
            }
        }

        // DELETE: api/Solicitudes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var solicitud = await _solicitudService.ObtenerPorIdAsync(id);
                if (solicitud == null) return NotFound(new { mensaje = "Solicitud no encontrada." });

                await _solicitudService.EliminarAsync(id);
                return Ok(new { mensaje = "Solicitud eliminada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar la solicitud.", detalle = ex.Message });
            }
        }

        // PUT: api/Solicitudes/5 
        [HttpPut("{id}")]
        public async Task<IActionResult> Modificar(int id, Solicitud solicitud)
        {
            if (id != solicitud.Id)
            {
                return BadRequest(new { mensaje = "El ID de la URL no coincide con el cuerpo." });
            }

            try
            {
                await _solicitudService.ModificarSolicitudAsync(id, solicitud);
                return Ok(new { mensaje = "Solicitud actualizada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno al editar.", detalle = ex.Message });
            }
        }

        // POST: api/Solicitudes/5/foto 
        [HttpPost("{id}/foto")]
        public async Task<IActionResult> SubirEvidencia(int id, IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0) return BadRequest("No se envió imagen.");

            try
            {
                var solicitud = await _solicitudService.ObtenerPorIdAsync(id);
                if (solicitud == null) return NotFound("Solicitud no encontrada.");

                // Convertir IFormFile a byte[]
                using (var memoryStream = new MemoryStream())
                {
                    await archivo.CopyToAsync(memoryStream);
                    solicitud.FotoEvidencia = memoryStream.ToArray();
                    solicitud.FotoEvidenciaTipo = archivo.ContentType;
                }

                // Guardamos usando el mismo método Modificar
                await _solicitudService.ModificarSolicitudAsync(id, solicitud);

                return Ok(new { mensaje = "Evidencia subida correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al subir la evidencia.", detalle = ex.Message });
            }
        }

        // GET: api/Solicitudes/5/foto 
        [HttpGet("{id}/foto")]
        public async Task<IActionResult> ObtenerEvidencia(int id)
        {
            try
            {
                var solicitud = await _solicitudService.ObtenerPorIdAsync(id);

                if (solicitud == null || solicitud.FotoEvidencia == null)
                {
                
                    return NotFound();
                }

                // Devolvemos el archivo binario para que el navegador lo muestre
                return File(solicitud.FotoEvidencia, solicitud.FotoEvidenciaTipo ?? "image/jpeg");
            }
            catch
            {
                return NotFound();
            }
        }
    }
}