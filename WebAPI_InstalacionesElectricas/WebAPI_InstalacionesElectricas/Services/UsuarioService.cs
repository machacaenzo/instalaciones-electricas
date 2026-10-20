using Microsoft.EntityFrameworkCore;
using WebAPI_InstalacionesElectricas.Data;
using WebAPI_InstalacionesElectricas.Models;
using WebAPI_InstalacionesElectricas.Utilidades;

namespace WebAPI_InstalacionesElectricas.Services
{
    public class UsuarioService
    {
        private readonly ApplicationDbContext _context;

        public UsuarioService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Obtener todos (Solo los Activos)
        public async Task<List<Usuario>> ObtenerTodosAsync(bool incluirInactivos = false)
        {
            var query = _context.Usuarios.Include(u => u.Rol).AsQueryable();

            if (!incluirInactivos)
            {
                query = query.Where(u => u.Activo == true);
            }

            return await query.ToListAsync();
        }

        // 2. Obtener por ID
        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == id && u.Activo == true);
        }

        // 3. Crear Usuario
        public async Task<Usuario> CrearAsync(Usuario usuario)
        {
            if (string.IsNullOrEmpty(usuario.Clave))
            {
                throw new Exception("La contraseña es obligatoria para registrarse.");
            }

            if (usuario.IdRol == 0)
            {
                var rolCliente = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Nombre == NombresRoles.Cliente);

                if (rolCliente != null)
                {
                    usuario.IdRol = rolCliente.Id;
                }
                else
                {
                    throw new Exception("Error Crítico: El rol 'Cliente' no existe en la Base de Datos.");
                }
            }

            // Encriptamos la clave
            usuario.Clave = Encriptador.EncriptarClave(usuario.Clave);

            // Guardamos
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        // 4. Modificar Usuario 
        public async Task ModificarAsync(int id, Usuario usuarioActualizado)
        {
            var usuarioExistente = await _context.Usuarios.FindAsync(id);

            
            if (usuarioExistente != null && usuarioExistente.Activo)
            {
                usuarioExistente.NombreCompleto = usuarioActualizado.NombreCompleto;
                usuarioExistente.Celular = usuarioActualizado.Celular;

                if (!string.IsNullOrEmpty(usuarioActualizado.Clave))
                {
                    usuarioExistente.Clave = Encriptador.EncriptarClave(usuarioActualizado.Clave);
                }

                
                if (usuarioActualizado.FotoPerfil != null && usuarioActualizado.FotoPerfil.Length > 0)
                {
                    usuarioExistente.FotoPerfil = usuarioActualizado.FotoPerfil;
                    usuarioExistente.FotoPerfilTipo = usuarioActualizado.FotoPerfilTipo;
                }

                
                if (usuarioActualizado.IdRol != 0 && usuarioActualizado.IdRol != usuarioExistente.IdRol)
                {
                    usuarioExistente.IdRol = usuarioActualizado.IdRol;
                }

                await _context.SaveChangesAsync();
            }
        }

        // 5. Eliminar (Baja Lógica)
        public async Task EliminarAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                usuario.Activo = false;
                await _context.SaveChangesAsync();
            }
        }

        // 6. Login
        public async Task<Usuario?> IniciarSesionAsync(string correo, string claveTextoPlano)
        {
            string claveEncriptada = Encriptador.EncriptarClave(claveTextoPlano);

            return await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == correo && u.Clave == claveEncriptada && u.Activo == true);
        }

        // 7. Reactivar
        public async Task<bool> ReactivarUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario != null)
            {
                usuario.Activo = true;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}