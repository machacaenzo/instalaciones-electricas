using Microsoft.EntityFrameworkCore;
using WebAPI_InstalacionesElectricas.Data;
using WebAPI_InstalacionesElectricas.Models;

namespace WebAPI_InstalacionesElectricas.Services
{
    public class RolService
    {
        private readonly ApplicationDbContext _context;

        public RolService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Rol>> ObtenerTodosAsync()
        {
            return await _context.Roles.ToListAsync();
        }
    }
}