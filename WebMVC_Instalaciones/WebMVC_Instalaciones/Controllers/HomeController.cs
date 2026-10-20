using Microsoft.AspNetCore.Mvc;

namespace WebMVC_Instalaciones.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/Index (La página principal)
        public IActionResult Index()
        {
           
            return View();
        }

        // GET: /Home/Error (Por si explota algo)
        public IActionResult Error()
        {
            return View();
        }
    }
}