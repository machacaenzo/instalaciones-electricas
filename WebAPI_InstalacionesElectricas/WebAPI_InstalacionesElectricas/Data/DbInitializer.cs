using WebAPI_InstalacionesElectricas.Models;
using WebAPI_InstalacionesElectricas.Utilidades;

namespace WebAPI_InstalacionesElectricas.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Crea la BD si no existe (vital para la nube)
            context.Database.EnsureCreated();

            // ==========================================
            // 1. ROLES
            // ==========================================
            if (!context.Roles.Any())
            {
                var roles = new Rol[]
                {
                    new Rol { Nombre = NombresRoles.Admin },      // ID 1
                    new Rol { Nombre = NombresRoles.Cliente },    // ID 2
                    new Rol { Nombre = NombresRoles.Instalador }  // ID 3
                };
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }

            // ==========================================
            // 2. ESTADOS (CORREGIDO SEGÚN TU IMAGEN)
            // ==========================================
            if (!context.EstadosSolicitudes.Any())
            {
                var estados = new EstadoSolicitud[]
                {
                    // ID 1: Cuando el cliente crea la solicitud
                    new EstadoSolicitud { Descripcion = "Pendiente" },
                    
                    // ID 2: Cuando el Admin le pone técnico (FALTABA ESTE)
                    new EstadoSolicitud { Descripcion = "Asignada" },
                    
                    // ID 3: Cuando el técnico le da "INICIAR"
                    new EstadoSolicitud { Descripcion = "En Proceso" }, 
                    
                    // ID 4: Cuando el técnico le da "FINALIZAR"
                    new EstadoSolicitud { Descripcion = "Finalizada" },
                    
                    // ID 5: Cuando se cancela (manual o automáticamente)
                    new EstadoSolicitud { Descripcion = "Cancelada" }
                };
                context.EstadosSolicitudes.AddRange(estados);
                context.SaveChanges();
            }
        }
    }
}