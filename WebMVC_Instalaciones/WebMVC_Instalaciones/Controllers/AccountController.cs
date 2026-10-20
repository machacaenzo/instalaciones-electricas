using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebMVC_Instalaciones.Models;
using WebMVC_Instalaciones.Services;

namespace WebMVC_Instalaciones.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;
        private readonly UsuarioService _usuarioService;

       
        public AccountController(AuthService authService, UsuarioService usuarioService)
        {
            _authService = authService;
            _usuarioService = usuarioService;
        }

        // ---------------- REGISTRO  ----------------

        [HttpGet]
        public IActionResult Register()
        {
            // Si ya está logueado, lo mandamos al inicio
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UsuarioRegistro usuarioRegistro)
        {
          
            if (!ModelState.IsValid) return View(usuarioRegistro);

            
            var exito = await _authService.RegistrarUsuario(usuarioRegistro);

            if (exito)
            {
               
                TempData["Mensaje"] = "¡Registro exitoso! Por favor inicia sesión.";
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Error = "No se pudo registrar. Es posible que el correo ya exista.";
                return View(usuarioRegistro);
            }
        }

        // ---------------- LOGIN ----------------

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UsuarioLogin usuarioLogin)
        {
            if (!ModelState.IsValid) return View(usuarioLogin);

           
            var usuarioEncontrado = await _authService.Login(usuarioLogin);

            if (usuarioEncontrado != null)
            {
                // --- CREACIÓN DE LA COOKIE  ---

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuarioEncontrado.NombreCompleto), 
                    new Claim(ClaimTypes.Email, usuarioEncontrado.Correo),        
                    new Claim(ClaimTypes.Role, usuarioEncontrado.NombreRol),     
                    new Claim("IdUsuario", usuarioEncontrado.Id.ToString())      
                };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

              
                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));

                
                TempData["Bienvenida"] = $"Hola, {usuarioEncontrado.NombreCompleto}. Acceso autorizado.";

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View(usuarioLogin);
            }
        }
        // ---------------- GESTIÓN DE PERFIL ----------------

        [HttpGet]
        [Authorize] 
        public async Task<IActionResult> Perfil()
        {
           
            var idClaim = User.FindFirst("IdUsuario");
            if (idClaim == null) return RedirectToAction("Login");

            int id = int.Parse(idClaim.Value);

           
            var usuario = await _usuarioService.ObtenerPorId(id);

            if (usuario == null) return RedirectToAction("Login");

            return View(usuario);
        }

        [HttpPost]
        [Authorize]
       
        [HttpPost]
        [Authorize]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Perfil(Usuario usuario)
        {
            
            ModelState.Remove("Clave");
            ModelState.Remove("ConfirmarClave");

            
            var idClaim = User.FindFirst("IdUsuario");
            if (idClaim == null) return RedirectToAction("Login");

            int idReal = int.Parse(idClaim.Value);
            usuario.Id = idReal;

           
            var exito = await _usuarioService.ActualizarUsuario(usuario);

           
            if (exito)
            {
                if (usuario.ArchivoFoto != null && usuario.ArchivoFoto.Length > 0)
                {
                   
                    await _usuarioService.SubirFotoPerfil(usuario.Id, usuario.ArchivoFoto);

                  
                    TempData["FotoActualizada"] = DateTime.Now.Ticks.ToString();
                }

               
                string rolActual = User.FindFirst(ClaimTypes.Role)?.Value ?? "Cliente";

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.NombreCompleto),
            new Claim(ClaimTypes.Email, usuario.Correo),
            new Claim(ClaimTypes.Role, rolActual),
            new Claim("IdUsuario", usuario.Id.ToString())
        };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));
                

                TempData["Mensaje"] = "Perfil actualizado correctamente.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "No se pudieron guardar los cambios.";
                return View(usuario);
            }
        }

        // ---------------- CERRAR SESIÓN ----------------
        public async Task<IActionResult> Logout()
        {
            // Borra la cookie del navegador
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index", "Home");
        }
    }
}