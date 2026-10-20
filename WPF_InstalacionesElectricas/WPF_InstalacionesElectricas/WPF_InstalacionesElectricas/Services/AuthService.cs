using System.Threading.Tasks;
using WPF_InstalacionesElectricas.Models;

namespace WPF_InstalacionesElectricas.Services
{
    public class AuthService
    {
        public async Task<Usuario> Login(string correo, string clave)
        {
            
            var loginData = new { Correo = correo, Clave = clave };

            return await ApiClient.PostAsync<Usuario>("Usuarios/login", loginData);
        }
    }
}