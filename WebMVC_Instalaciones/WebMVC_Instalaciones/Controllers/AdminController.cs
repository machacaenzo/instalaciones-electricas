using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMVC_Instalaciones.Models;
using WebMVC_Instalaciones.Services;

namespace WebMVC_Instalaciones.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly SolicitudService _solicitudService;
        private readonly UsuarioService _usuarioService;
        private readonly AsignacionService _asignacionService;

        public AdminController(SolicitudService solicitudService, UsuarioService usuarioService, AsignacionService asignacionService)
        {
            _solicitudService = solicitudService;
            _usuarioService = usuarioService;
            _asignacionService = asignacionService;
        }

        // SECCIÓN 1: DASHBOARD (Resumen General)
        
        #region DASHBOARD

        // GET: Dashboard Principal
        public async Task<IActionResult> Dashboard()
        {
            // 1. Traer datos crudos de la API
            var todasSolicitudes = await _solicitudService.ObtenerTodas();
            var todosUsuarios = await _usuarioService.ObtenerTodos(true); // true = incluir inactivos

            // 2. Calcular Totales
            int totalSoli = todasSolicitudes.Count;
            // Evitamos división por cero si no hay datos
            if (totalSoli == 0) totalSoli = 1;

            var vm = new Dashboard
            {
                // Contadores
                CantidadPendientes = todasSolicitudes.Count(s => s.IdEstadoSolicitud == 1),
                CantidadEnProceso = todasSolicitudes.Count(s => s.IdEstadoSolicitud == 2),
                CantidadFinalizadas = todasSolicitudes.Count(s => s.IdEstadoSolicitud == 3),

                TotalUsuarios = todosUsuarios.Count,
                TotalTecnicos = todosUsuarios.Count(u => u.IdRol == 3 || u.NombreRol == "Instalador"),

                // Lista recortada (Las 5 más nuevas)
                UltimasSolicitudes = todasSolicitudes
                    .OrderByDescending(s => s.FechaSolicitud) // Asumiendo que tenés fecha, si no usa ID
                    .Take(5)
                    .ToList()
            };

            // 3. Calcular Porcentajes para las barras
            vm.PorcentajePendientes = (vm.CantidadPendientes * 100) / totalSoli;
            vm.PorcentajeFinalizadas = (vm.CantidadFinalizadas * 100) / totalSoli;

            return View(vm);
        }

        #endregion

        
        // SECCIÓN 2: GESTIÓN DE USUARIOS (ABM Completo)
       
        #region GESTIÓN DE USUARIOS

        // 1. LISTAR (Con filtro de inactivos)
        [HttpGet]
        public async Task<IActionResult> Usuarios(bool mostrarInactivos = false)
        {
          
            var lista = await _usuarioService.ObtenerTodos(mostrarInactivos);

            ViewBag.MostrarInactivos = mostrarInactivos;
            return View("ListaUsuarios", lista);
        }

        // 2. CREAR 
        [HttpGet]
        public IActionResult CrearUsuario()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario(UsuarioRegistro usuarioRegistro)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Por favor corregí los errores del formulario.";
                return View(usuarioRegistro);
            }

            var exito = await _usuarioService.CrearUsuario(usuarioRegistro);

            if (exito)
            {
                TempData["Mensaje"] = "Usuario registrado correctamente.";
                return RedirectToAction("Usuarios");
            }
            else
            {
                ViewBag.Error = "Error al crear. El correo podría estar duplicado.";
                return View(usuarioRegistro);
            }
        }

        // 3. EDITAR 
        [HttpGet]
        public async Task<IActionResult> EditarUsuario(int id)
        {
            var usuario = await _usuarioService.ObtenerPorId(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> EditarUsuario(Usuario usuario)
        {
            // Validamos manualmente lo esencial
            if (string.IsNullOrEmpty(usuario.NombreCompleto) || string.IsNullOrEmpty(usuario.Correo))
            {
                ViewBag.Error = "Nombre y Correo son obligatorios.";
                return View(usuario);
            }

            var exito = await _usuarioService.ActualizarUsuario(usuario);

            if (exito)
            {
                TempData["Mensaje"] = "Datos actualizados correctamente.";
                return RedirectToAction("Usuarios");
            }
            else
            {
                ViewBag.Error = "No se pudieron guardar los cambios.";
                return View(usuario);
            }
        }

        // 4. ELIMINAR 
        [HttpPost]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var exito = await _usuarioService.EliminarUsuario(id);

            if (exito) TempData["Mensaje"] = "Usuario dado de baja.";
            else TempData["Error"] = "No se pudo eliminar.";

            return RedirectToAction("Usuarios");
        }

        // 5. REACTIVAR
        [HttpPost]
        public async Task<IActionResult> ReactivarUsuario(int id)
        {
            var exito = await _usuarioService.ReactivarUsuario(id);

            if (exito) TempData["Mensaje"] = "Usuario reactivado exitosamente.";
            else TempData["Error"] = "No se pudo reactivar.";

           
            return RedirectToAction("Usuarios", new { mostrarInactivos = true });
        }

        #endregion

        
        // SECCIÓN 3: GESTIÓN DE SOLICITUDES (Asignaciones)
       
        #region GESTIÓN DE SOLICITUDES

        // 1. LISTAR
        public async Task<IActionResult> Solicitudes()
        {
            var lista = await _solicitudService.ObtenerTodas();
            return View("ListaSolicitudes", lista);
        }

        // 2. ASIGNAR TÉCNICO
        [HttpGet]
        public async Task<IActionResult> Asignar(int idSolicitud)
        {
            var todas = await _solicitudService.ObtenerTodas();
            var solicitud = todas.FirstOrDefault(s => s.Id == idSolicitud);

            if (solicitud == null) return NotFound();

            
            var usuarios = await _usuarioService.ObtenerTodos(true);

           
            var tecnicos = usuarios
                .Where(u => (u.IdRol == 3 || u.NombreRol == "Instalador") && u.Activo)
                .ToList();

            ViewBag.Tecnicos = tecnicos;

            return View(solicitud);
        }

        // 3. CONFIRMAR ASIGNACIÓN 
        [HttpPost]
        public async Task<IActionResult> ConfirmarAsignacion(int idSolicitud, int idTecnico)
        {
            var exito = await _asignacionService.AsignarTecnico(idSolicitud, idTecnico);

            if (exito)
            {
                TempData["Mensaje"] = "¡Técnico asignado correctamente!";
                return RedirectToAction("Solicitudes");
            }
            else
            {
                TempData["Error"] = "No se pudo asignar. Verifique si el técnico está disponible.";
                return RedirectToAction("Solicitudes");
            }
        }

        #endregion


    }
}