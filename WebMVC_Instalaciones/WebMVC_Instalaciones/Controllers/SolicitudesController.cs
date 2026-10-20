using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebMVC_Instalaciones.Models;
using WebMVC_Instalaciones.Services;

namespace WebMVC_Instalaciones.Controllers
{
    [Authorize] 
    public class SolicitudesController : Controller
    {
        private readonly SolicitudService _solicitudService;

        public SolicitudesController(SolicitudService solicitudService)
        {
            _solicitudService = solicitudService;
        }

       
        // ÁREA CLIENTE
       

        // 1. LISTADO
        public async Task<IActionResult> Index()
        {
            var idUsuarioStr = User.FindFirst("IdUsuario")?.Value;
            if (string.IsNullOrEmpty(idUsuarioStr)) return RedirectToAction("Login", "Account");

            int idUsuario = int.Parse(idUsuarioStr);
            var lista = await _solicitudService.ObtenerPorCliente(idUsuario);

            return View(lista);
        }

        // 2. CREAR 
        public IActionResult Crear()
        {
            return View();
        }

        // 3. CREAR 
        [HttpPost]
        public async Task<IActionResult> Crear(Solicitud solicitud)
        {
            
            ModelState.Remove("Cliente");
            ModelState.Remove("Estado");

            if (!ModelState.IsValid) return View(solicitud);

            
            var idUsuarioStr = User.FindFirst("IdUsuario")?.Value;
            solicitud.IdUsuario = int.Parse(idUsuarioStr!);

            solicitud.IdEstadoSolicitud = 1;
            solicitud.Activo = true;
            solicitud.FechaSolicitud = DateTime.Now;

           
            int idNuevaSolicitud = await _solicitudService.CrearSolicitud(solicitud);

            if (idNuevaSolicitud > 0)
            {
                
                if (solicitud.ArchivoFoto != null && solicitud.ArchivoFoto.Length > 0)
                {
                    await _solicitudService.SubirFotoEvidencia(idNuevaSolicitud, solicitud.ArchivoFoto);
                }

                TempData["Mensaje"] = "¡Solicitud creada con éxito!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Ocurrió un error al enviar la solicitud.";
                return View(solicitud);
            }
        }

        // 4. EDITAR 
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var solicitud = await _solicitudService.ObtenerPorId(id);
            if (solicitud == null) return NotFound();

            
            var idUsuario = int.Parse(User.FindFirst("IdUsuario")!.Value);

            if (solicitud.IdUsuario != idUsuario || solicitud.IdEstadoSolicitud != 1)
            {
                TempData["Error"] = "No se puede editar esta solicitud.";
                return RedirectToAction("Index");
            }

            return View(solicitud);
        }

        // 5. EDITAR (Guardar cambios)
        
        [HttpPost]
        public async Task<IActionResult> Editar(Solicitud solicitud)
        {
          
            ModelState.Remove("Cliente");
            ModelState.Remove("Estado");

            if (!ModelState.IsValid) return View(solicitud);

            
            var exito = await _solicitudService.EditarSolicitud(solicitud);

            if (exito)
            {
               
                if (solicitud.ArchivoFoto != null && solicitud.ArchivoFoto.Length > 0)
                {
                    await _solicitudService.SubirFotoEvidencia(solicitud.Id, solicitud.ArchivoFoto);
                }

                TempData["Mensaje"] = "Solicitud actualizada correctamente.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "No se pudo actualizar la solicitud.";
                return View(solicitud);
            }
        }

        // 6. CANCELAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var idUsuario = int.Parse(User.FindFirst("IdUsuario")!.Value);
            var misSolicitudes = await _solicitudService.ObtenerPorCliente(idUsuario);
            var solicitud = misSolicitudes.FirstOrDefault(s => s.Id == id);

            if (solicitud == null)
            {
                TempData["Error"] = "No se encontró la solicitud.";
                return RedirectToAction("Index");
            }

            if (solicitud.IdEstadoSolicitud != 1)
            {
                TempData["Error"] = "No se puede cancelar una solicitud en proceso.";
                return RedirectToAction("Index");
            }

            var exito = await _solicitudService.EliminarSolicitud(id);

            if (exito) TempData["Mensaje"] = "Solicitud cancelada.";
            else TempData["Error"] = "Error al cancelar.";

            return RedirectToAction("Index");
        }

        
        // ÁREA INSTALADOR
      

        // 7. HOJA DE RUTA
        [Authorize(Roles = "Instalador")]
        public async Task<IActionResult> MisTrabajos()
        {
            var idUsuarioStr = User.FindFirst("IdUsuario")?.Value;
            if (string.IsNullOrEmpty(idUsuarioStr)) return RedirectToAction("Login", "Account");

            int idInstalador = int.Parse(idUsuarioStr);

            var lista = await _solicitudService.ObtenerPorInstalador(idInstalador);

            return View(lista);
        }
    }
}