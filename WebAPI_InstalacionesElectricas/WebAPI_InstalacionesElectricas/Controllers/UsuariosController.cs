using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Necesario para capturar errores de BD
using WebAPI_InstalacionesElectricas.Models;
using WebAPI_InstalacionesElectricas.Services;

namespace WebAPI_InstalacionesElectricas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public UsuariosController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioSinClave>>> ObtenerTodos([FromQuery] bool incluirInactivos = false)
        {
            try
            {
                var usuarios = await _usuarioService.ObtenerTodosAsync(incluirInactivos);

                var usuariosDTO = usuarios.Select(u => new UsuarioSinClave
                {
                    Id = u.Id,
                    NombreCompleto = u.NombreCompleto,
                    Correo = u.Correo,
                    Celular = u.Celular,
                    IdRol = u.IdRol,
                    NombreRol = u.Rol?.Nombre ?? "Sin Rol",
                    Activo = u.Activo
                }).ToList();

                return Ok(usuariosDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno.", detalle = ex.Message });
            }
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioSinClave>> ObtenerPorId(int id)
        {
            try
            {
                var usuario = await _usuarioService.ObtenerPorIdAsync(id);

                if (usuario == null)
                {
                    return NotFound(new { mensaje = $"No se encontró ningún usuario con el ID {id}." });
                }

                var usuarioDTO = new UsuarioSinClave
                {
                    Id = usuario.Id,
                    NombreCompleto = usuario.NombreCompleto,
                    Correo = usuario.Correo,
                    Celular = usuario.Celular,
                    IdRol = usuario.IdRol,
                    NombreRol = usuario.Rol?.Nombre ?? "Sin Rol",
                    Activo = usuario.Activo
                };

                return Ok(usuarioDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al buscar el usuario.", detalle = ex.Message });
            }
        }

        // POST: api/Usuarios

        [HttpPost]
        public async Task<ActionResult<UsuarioSinClave>> Crear(Usuario usuario)
        {
            try
            {
                await _usuarioService.CrearAsync(usuario);

                var usuarioDTO = new UsuarioSinClave
                {
                    Id = usuario.Id,
                    NombreCompleto = usuario.NombreCompleto,
                    Correo = usuario.Correo,
                    Celular = usuario.Celular,
                    IdRol = usuario.IdRol,
                    NombreRol = usuario.Rol?.Nombre ?? null,
                    Activo = usuario.Activo
                };

                return CreatedAtAction(nameof(ObtenerPorId), new { id = usuario.Id }, usuarioDTO);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_Usuarios_Correo"))
                {
                    return Conflict(new { mensaje = "El correo electrónico ya está registrado. Por favor, use otro." });
                }
                return BadRequest(new { mensaje = "Error al guardar en la base de datos.", detalle = ex.InnerException?.Message ?? ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error inesperado.", detalle = ex.Message });
            }
        }

        // POST: api/Usuarios/login
        [HttpPost("login")]
        public async Task<ActionResult<UsuarioSinClave>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var usuario = await _usuarioService.IniciarSesionAsync(request.Correo, request.Clave);

                if (usuario == null)
                {
                    return Unauthorized(new { mensaje = "Credenciales incorrectas o usuario inactivo." });
                }

                var usuarioDTO = new UsuarioSinClave
                {
                    Id = usuario.Id,
                    NombreCompleto = usuario.NombreCompleto,
                    Correo = usuario.Correo,
                    Celular = usuario.Celular,
                    IdRol = usuario.IdRol,
                    NombreRol = usuario.Rol?.Nombre ?? "Sin Rol",
                    Activo = usuario.Activo
                };

                return Ok(usuarioDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al intentar iniciar sesión.", detalle = ex.Message });
            }
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Modificar(int id, Usuario usuario)
        {
            if (id != usuario.Id)
                return BadRequest(new { mensaje = "El ID de la URL no coincide con el ID del usuario." });

            try
            {
                await _usuarioService.ModificarAsync(id, usuario);
                return Ok(new { mensaje = "Usuario actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el usuario.", detalle = ex.Message });
            }
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var usuario = await _usuarioService.ObtenerPorIdAsync(id);
                if (usuario == null)
                {
                    return NotFound(new { mensaje = "El usuario no existe o ya fue eliminado." });
                }

                await _usuarioService.EliminarAsync(id);
                return Ok(new { mensaje = "Usuario eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el usuario.", detalle = ex.Message });
            }
        }

        // PUT: api/Usuarios/5/reactivar
        [HttpPut("{id}/reactivar")]
        public async Task<IActionResult> Reactivar(int id)
        {
            try
            {
                bool resultado = await _usuarioService.ReactivarUsuarioAsync(id);

                if (!resultado)
                {
                    return NotFound(new { mensaje = "No se encontró el usuario para reactivar." });
                }

                return Ok(new { mensaje = "Usuario reactivado exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al reactivar el usuario.", detalle = ex.Message });
            }
        }


        // POST: api/Usuarios/5/foto
        [HttpPost("{id}/foto")]
        public async Task<IActionResult> SubirFotoPerfil(int id, IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0) return BadRequest("No se envió imagen.");

            try
            {
                var usuario = await _usuarioService.ObtenerPorIdAsync(id);
                if (usuario == null) return NotFound("Usuario no encontrado.");

                // Convertir IFormFile a byte[]
                using (var memoryStream = new MemoryStream())
                {
                    await archivo.CopyToAsync(memoryStream);
                    usuario.FotoPerfil = memoryStream.ToArray();
                    usuario.FotoPerfilTipo = archivo.ContentType; 
                }

                
                await _usuarioService.ModificarAsync(id, usuario);

                return Ok(new { mensaje = "Foto de perfil actualizada." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al subir la foto.", detalle = ex.Message });
            }
        }

        // GET: api/Usuarios/5/foto
        [HttpGet("{id}/foto")]
        public async Task<IActionResult> ObtenerFotoPerfil(int id)
        {
            try
            {
                var usuario = await _usuarioService.ObtenerPorIdAsync(id);

                if (usuario == null || usuario.FotoPerfil == null)
                {
                    return NotFound(); 
                }

                // Devolvemos el archivo binario para que el navegador lo muestre
                return File(usuario.FotoPerfil, usuario.FotoPerfilTipo ?? "image/jpeg");
            }
            catch
            {
                return NotFound();
            }
        }
    }

    public class LoginRequest
    {
        public string Correo { get; set; }
        public string Clave { get; set; }
    }
}