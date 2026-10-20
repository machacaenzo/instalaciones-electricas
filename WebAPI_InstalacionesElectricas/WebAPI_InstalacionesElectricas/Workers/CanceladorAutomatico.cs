using Microsoft.EntityFrameworkCore;
using WebAPI_InstalacionesElectricas.Data;

namespace WebAPI_InstalacionesElectricas.Workers
{
    public class CanceladorAutomatico : BackgroundService
    {
        // Necesitamos esto para poder "crear" el DbContext dentro de un proceso en segundo plano
        private readonly IServiceProvider _serviceProvider;

        public CanceladorAutomatico(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Este bucle se ejecuta mientras la API esté encendida
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // 1. Crear un Scope (un ámbito temporal para usar la BD)
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        // 2. BUSCAR LAS SOLICITUDES VENCIDAS
                        // Criterio:
                        // - FechaDeseada es MENOR a Hoy (ya pasó)
                        // - Estado es "Pendiente" (ID 2) o "Asignada" (ID 1? Revisa tus IDs)
                        // - NO tocar las que ya están "Finalizada" (ID 4) o "Cancelada" (ID 5)

                        var fechaLimite = DateTime.Now.Date; // Hoy a las 00:00

                        var vencidas = await context.Solicitudes
                            .Where(s => s.FechaDeseada < fechaLimite
                                     && s.IdEstadoSolicitud != 4 // No Finalizada
                                     && s.IdEstadoSolicitud != 5) // No Cancelada
                            .ToListAsync(stoppingToken);

                        if (vencidas.Count > 0)
                        {
                            // 3. CAMBIAR A CANCELADA (ID 5)
                            foreach (var solicitud in vencidas)
                            {
                                solicitud.IdEstadoSolicitud = 5; // 5 = Cancelada
                                // Opcional: Agregar un comentario automático en la descripción
                                solicitud.Descripcion += " [CANCELADA AUTOMÁTICAMENTE POR VENCIMIENTO]";
                            }

                            // 4. GUARDAR CAMBIOS
                            await context.SaveChangesAsync(stoppingToken);

                            Console.WriteLine($"[AUTO-CANCEL] Se cancelaron {vencidas.Count} solicitudes vencidas.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR WORKER] {ex.Message}");
                }

                // 5. DORMIR EL ROBOT (Espera)
                // Aquí defines cada cuánto revisa. 
                // Para pruebas ponle: TimeSpan.FromMinutes(1)
                // Para producción: TimeSpan.FromHours(1) o TimeSpan.FromDays(1)
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}