using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using WebMVC_Instalaciones.Services;

namespace WebMVC_Instalaciones.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReportesController : Controller
    {
        private readonly SolicitudService _solicitudService;
        private readonly UsuarioService _usuarioService;

        public ReportesController(SolicitudService solicitudService, UsuarioService usuarioService)
        {
            _solicitudService = solicitudService;
            _usuarioService = usuarioService;
        }

        
        // GET: Vista con Gráficos 
        public async Task<IActionResult> Index(DateTime? desde, DateTime? hasta, int? idTecnico, int? idCliente)
        {
           
            var solicitudes = await _solicitudService.ObtenerTodas();
            var usuarios = await _usuarioService.ObtenerTodos(true);

           
            ViewBag.Tecnicos = usuarios
                .Where(u => u.IdRol == 3 || u.NombreRol == "Instalador")
                .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.NombreCompleto, Selected = u.Id == idTecnico })
                .ToList();

            ViewBag.Clientes = usuarios
                .Where(u => u.IdRol == 2 || u.NombreRol == "Cliente")
                .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.NombreCompleto, Selected = u.Id == idCliente })
                .ToList();

            // 3. APLICAR FILTROS 

            
            if (desde.HasValue) solicitudes = solicitudes.Where(s => s.FechaSolicitud.Date >= desde.Value.Date).ToList();
            if (hasta.HasValue) solicitudes = solicitudes.Where(s => s.FechaSolicitud.Date <= hasta.Value.Date).ToList();

           
            if (idCliente.HasValue)
                solicitudes = solicitudes.Where(s => s.IdUsuario == idCliente.Value).ToList();

            
            if (idTecnico.HasValue)
            {
                
                solicitudes = solicitudes
                    .Where(s => s.Asignaciones != null && s.Asignaciones.Any(a => a.IdInstalador == idTecnico.Value))
                    .ToList();
            }

           
            ViewBag.FechaDesde = desde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = hasta?.ToString("yyyy-MM-dd");
            ViewBag.IdTecnico = idTecnico;
            ViewBag.IdCliente = idCliente;

            
            ViewBag.Pendientes = solicitudes.Count(s => s.IdEstadoSolicitud == 1);
            ViewBag.Asignadas = solicitudes.Count(s => s.IdEstadoSolicitud == 2);
            ViewBag.EnProceso = solicitudes.Count(s => s.IdEstadoSolicitud == 3);
            ViewBag.Finalizadas = solicitudes.Count(s => s.IdEstadoSolicitud == 4);
            ViewBag.Canceladas = solicitudes.Count(s => s.IdEstadoSolicitud == 5);

          
            var ranking = solicitudes
                .Where(s => s.Cliente != null)
                .GroupBy(s => s.Cliente?.NombreCompleto ?? "Anónimo")
                .Select(g => new { Nombre = g.Key, Cantidad = g.Count() })
                .OrderByDescending(x => x.Cantidad)
                .Take(5)
                .ToList();

           
            if (ranking.Any())
            {
                ViewBag.LabelsRanking = ranking.Select(r => r.Nombre).ToArray();
                ViewBag.DataRanking = ranking.Select(r => r.Cantidad).ToArray();
            }
            else
            {
                ViewBag.LabelsRanking = new string[] { "Sin Datos" };
                ViewBag.DataRanking = new int[] { 0 };
            }

            return View();
        }

        // GET: Descargar PDF
        public async Task<IActionResult> ExportarPDF(DateTime? desde, DateTime? hasta, int? idTecnico, int? idCliente)
        {
            var solicitudes = await _solicitudService.ObtenerTodas();

           
            if (desde.HasValue) solicitudes = solicitudes.Where(s => s.FechaSolicitud.Date >= desde.Value.Date).ToList();
            if (hasta.HasValue) solicitudes = solicitudes.Where(s => s.FechaSolicitud.Date <= hasta.Value.Date).ToList();
            if (idCliente.HasValue) solicitudes = solicitudes.Where(s => s.IdUsuario == idCliente).ToList();
            if (idTecnico.HasValue) solicitudes = solicitudes.Where(s => s.Asignaciones != null && s.Asignaciones.Any(a => a.IdInstalador == idTecnico)).ToList();

            var listaOrdenada = solicitudes.OrderByDescending(x => x.FechaSolicitud).ToList();

            return new ViewAsPdf("VistaPDF", listaOrdenada)
            {
                FileName = $"Reporte_{DateTime.Now:yyyyMMdd}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                CustomSwitches = "--disable-smart-shrinking"
            };
        }
    }
}