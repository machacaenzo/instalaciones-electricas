using Microsoft.EntityFrameworkCore;
using WebAPI_InstalacionesElectricas.Data;
using WebAPI_InstalacionesElectricas.Models;
using WebAPI_InstalacionesElectricas.Utilidades; // Importante para NombresEstados y NombresRoles

namespace WebAPI_InstalacionesElectricas.Services
{
    public class AsignacionService
    {
        private readonly ApplicationDbContext _context;

        public AsignacionService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Asignar un técnico a una solicitud (Lógica Blindada sin IDs fijos)
        public async Task AsignarTecnicoAsync(Asignacion asignacion)
        {
            // -----------------------------------------------------------
            // 1. OBTENER IDs DINÁMICAMENTE (Para no usar números fijos)
            // -----------------------------------------------------------

            // Buscamos el ID del rol "Instalador"
            var rolInstalador = await _context.Roles
                .FirstOrDefaultAsync(r => r.Nombre == NombresRoles.Instalador);

            if (rolInstalador == null)
                throw new Exception("Error Crítico: El rol 'Instalador' no está configurado en la BD.");

            // Buscamos los IDs de los estados críticos
            var estadoFinalizada = await _context.EstadosSolicitudes
                .FirstOrDefaultAsync(e => e.Descripcion == NombresEstados.Finalizada);
            var estadoCancelada = await _context.EstadosSolicitudes
                .FirstOrDefaultAsync(e => e.Descripcion == NombresEstados.Cancelada);
            var estadoAsignada = await _context.EstadosSolicitudes
                .FirstOrDefaultAsync(e => e.Descripcion == NombresEstados.Asignada);

            if (estadoFinalizada == null || estadoCancelada == null || estadoAsignada == null)
                throw new Exception("Error Crítico: Los estados de solicitud no están configurados en la BD.");

            // -----------------------------------------------------------
            // 2. VALIDACIONES DE NEGOCIO
            // -----------------------------------------------------------

            // A. Validar Solicitud
            var solicitud = await _context.Solicitudes.FindAsync(asignacion.IdSolicitud);
            if (solicitud == null || !solicitud.Activo)
                throw new Exception("Solicitud no encontrada o eliminada.");

            // Regla: No asignar si ya terminó o se canceló
            if (solicitud.IdEstadoSolicitud == estadoFinalizada.Id ||
                solicitud.IdEstadoSolicitud == estadoCancelada.Id)
            {
                throw new Exception("No se puede asignar técnicos a una solicitud que ya está finalizada o cancelada.");
            }

            // B. Validar Técnico
            var tecnico = await _context.Usuarios.FindAsync(asignacion.IdInstalador);
            if (tecnico == null || !tecnico.Activo)
                throw new Exception("Técnico no encontrado.");

            // Regla: Tiene que ser Instalador (Usamos el ID que buscamos dinámicamente)
            if (tecnico.IdRol != rolInstalador.Id)
            {
                throw new Exception($"El usuario {tecnico.NombreCompleto} no tiene el rol de Instalador.");
            }

            // C. Validar Disponibilidad (Misma fecha)
            bool tieneTrabajoEseDia = await _context.Asignaciones
                .Include(a => a.Solicitud)
                .AnyAsync(a => a.IdInstalador == asignacion.IdInstalador &&
                               a.Solicitud.FechaDeseada.Date == solicitud.FechaDeseada.Date);

            if (tieneTrabajoEseDia)
            {
                throw new Exception($"El técnico {tecnico.NombreCompleto} ya tiene una instalación asignada para el día {solicitud.FechaDeseada.ToShortDateString()}.");
            }

            // -----------------------------------------------------------
            // 3. GUARDADO Y TRANSICIÓN DE ESTADO
            // -----------------------------------------------------------

            // Guardar la asignación
            _context.Asignaciones.Add(asignacion);

            // Cambiar estado a "Asignada" (Dinámico)
            solicitud.IdEstadoSolicitud = estadoAsignada.Id;

            await _context.SaveChangesAsync();
        }

        // Ver historial de asignaciones
        public async Task<List<Asignacion>> ObtenerTodasAsync()
        {
            return await _context.Asignaciones
                .Include(a => a.Instalador)
                .Include(a => a.Solicitud)
                .ToListAsync();
        }
    }
}