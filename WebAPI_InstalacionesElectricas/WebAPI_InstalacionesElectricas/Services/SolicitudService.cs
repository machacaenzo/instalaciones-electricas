using Microsoft.EntityFrameworkCore;
using WebAPI_InstalacionesElectricas.Data;
using WebAPI_InstalacionesElectricas.Models;

namespace WebAPI_InstalacionesElectricas.Services
{
    public class SolicitudService
    {
        private readonly ApplicationDbContext _context;

        public SolicitudService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Obtener TODAS
        public async Task<List<Solicitud>> ObtenerTodasAsync()
        {
            return await _context.Solicitudes
                .Where(s => s.Activo == true)
                .Include(s => s.Cliente)
                .Include(s => s.Estado)
                .Include(s => s.Asignaciones)
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();
        }

        // 2. Obtener por ID
        public async Task<Solicitud?> ObtenerPorIdAsync(int id)
        {
            return await _context.Solicitudes
                .Include(s => s.Cliente)
                .Include(s => s.Estado)
                .Include(s => s.Asignaciones)!
                    .ThenInclude(a => a.Instalador)
                .FirstOrDefaultAsync(s => s.Id == id && s.Activo == true);
        }

        // 3. Obtener por Cliente
        public async Task<List<Solicitud>> ObtenerPorClienteIdAsync(int idCliente)
        {
            return await _context.Solicitudes
                .Where(s => s.IdUsuario == idCliente && s.Activo == true)
                .Include(s => s.Estado)
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();
        }

        // 4. Obtener por Instalador
        public async Task<List<Solicitud>> ObtenerPorInstaladorIdAsync(int idInstalador)
        {
            return await _context.Solicitudes
                .Where(s => s.Asignaciones.Any(a => a.IdInstalador == idInstalador) && s.Activo == true)
                .Include(s => s.Cliente)
                .Include(s => s.Estado)
                .OrderBy(s => s.FechaDeseada)
                .ToListAsync();
        }

        // 5. Crear Solicitud
        public async Task<Solicitud> CrearAsync(Solicitud solicitud)
        {
            if (solicitud.IdEstadoSolicitud == 0) solicitud.IdEstadoSolicitud = 1;

            if (solicitud.FechaDeseada.Date < DateTime.Now.Date)
            {
                throw new Exception("La fecha deseada no puede ser anterior a hoy.");
            }

            _context.Solicitudes.Add(solicitud);
            await _context.SaveChangesAsync();
            return solicitud;
        }

        // 6. Cambiar Estado
        public async Task ActualizarEstadoAsync(int idSolicitud, int idNuevoEstado)
        {
            var solicitud = await _context.Solicitudes.FindAsync(idSolicitud);
            if (solicitud != null && solicitud.Activo)
            {
                solicitud.IdEstadoSolicitud = idNuevoEstado;
                await _context.SaveChangesAsync();
            }
        }

        // 7. Eliminar
        public async Task EliminarAsync(int id)
        {
            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud != null)
            {
                solicitud.Activo = false;
                await _context.SaveChangesAsync();
            }
        }

        // 8. MODIFICAR DATOS 
        public async Task ModificarSolicitudAsync(int id, Solicitud solicitudActualizada)
        {
            var solicitudExistente = await _context.Solicitudes.FindAsync(id);

            if (solicitudExistente != null && solicitudExistente.Activo)
            {
                solicitudExistente.Direccion = solicitudActualizada.Direccion;
                solicitudExistente.Descripcion = solicitudActualizada.Descripcion;
                solicitudExistente.FechaDeseada = solicitudActualizada.FechaDeseada;

                // NUEVO: ACTUALIZAR FOTO EVIDENCIA (Solo si viene una nueva)
                if (solicitudActualizada.FotoEvidencia != null && solicitudActualizada.FotoEvidencia.Length > 0)
                {
                    solicitudExistente.FotoEvidencia = solicitudActualizada.FotoEvidencia;
                    solicitudExistente.FotoEvidenciaTipo = solicitudActualizada.FotoEvidenciaTipo;
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("La solicitud no existe o fue eliminada.");
            }
        }
    }
}