using System.Text;
using System.Text.Json;
using WebMVC_Instalaciones.Models;

namespace WebMVC_Instalaciones.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            
            _baseUrl = "https://localhost:7088/api/Usuarios";
        }

       
        public async Task<bool> RegistrarUsuario(UsuarioRegistro datos)
        {
            var usuarioApi = new
            {
                NombreCompleto = datos.NombreCompleto,
                Correo = datos.Correo,
                Clave = datos.Clave,
                Celular = datos.Celular,
                IdRol = 0, // Rol automático
                Activo = true
            };

            var json = JsonSerializer.Serialize(usuarioApi);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_baseUrl, content);

            return response.IsSuccessStatusCode;
        }

       
        public async Task<Usuario?> Login(UsuarioLogin datos)
        {
            var loginApi = new
            {
                Correo = datos.Correo,
                Clave = datos.Clave
            };

            var json = JsonSerializer.Serialize(loginApi);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                return JsonSerializer.Deserialize<Usuario>(responseString, opciones);
            }

            return null;
        }

    }
}