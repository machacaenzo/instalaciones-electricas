using System.Collections.Generic;
using System.Threading.Tasks;
using WPF_InstalacionesElectricas.Models;

namespace WPF_InstalacionesElectricas.Services
{
    public class SolicitudService
    {
        // 1. Obtener lista de tareas
        public async Task<List<Solicitud>> ObtenerPorInstalador(int idInstalador)
        {
            
            var lista = await ApiClient.GetAsync<List<Solicitud>>($"Solicitudes/instalador/{idInstalador}");

           
            return lista ?? new List<Solicitud>();
        }

        // 2. Actualizar estado
        public async Task<bool> ActualizarEstado(int idSolicitud, int nuevoEstadoId)
        {
            
            return await ApiClient.PutAsync($"Solicitudes/{idSolicitud}/estado", nuevoEstadoId);
        }
    }
}