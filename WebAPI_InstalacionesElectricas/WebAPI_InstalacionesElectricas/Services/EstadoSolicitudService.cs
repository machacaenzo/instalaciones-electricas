using Microsoft.EntityFrameworkCore;
using WebAPI_InstalacionesElectricas.Data;
using WebAPI_InstalacionesElectricas.Models;

namespace WebAPI_InstalacionesElectricas.Services
{
    public class EstadoSolicitudService
    {
        private readonly ApplicationDbContext _context;

        public EstadoSolicitudService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EstadoSolicitud>> ObtenerTodosAsync()
        {
            return await _context.EstadosSolicitudes.ToListAsync();
        }
    }
}