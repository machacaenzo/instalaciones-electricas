using System.Text;
using System.Text.Json;
using WebMVC_Instalaciones.Models;

namespace WebMVC_Instalaciones.Services
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public UsuarioService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            
            _baseUrl = "https://localhost:7088/api/Usuarios";
        }

        // 1. OBTENER TODOS 
        public async Task<List<Usuario>> ObtenerTodos(bool incluirInactivos = false)
        {
            
            string url = incluirInactivos ? $"{_baseUrl}?incluirInactivos=true" : _baseUrl;

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<Usuario>>(json, opciones) ?? new List<Usuario>();
            }
            return new List<Usuario>();
        }

        // 2. OBTENER UNO POR ID 
        public async Task<Usuario?> ObtenerPorId(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<Usuario>(json, opciones);
            }
            return null;
        }

        // 3. CREAR USUARIO 
        public async Task<bool> CrearUsuario(UsuarioRegistro usuario)
        {
            var usuarioApi = new
            {
                NombreCompleto = usuario.NombreCompleto,
                Correo = usuario.Correo,
                Clave = usuario.Clave,
                Celular = usuario.Celular,
                IdRol = usuario.IdRol,
                Activo = true
            };

            var json = JsonSerializer.Serialize(usuarioApi);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_baseUrl, content);
            return response.IsSuccessStatusCode;
        }

        // 4. ACTUALIZAR 
        public async Task<bool> ActualizarUsuario(Usuario usuario)
        {
            var json = JsonSerializer.Serialize(usuario);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_baseUrl}/{usuario.Id}", content);

            return response.IsSuccessStatusCode;
        }

        // 5. ELIMINAR 
        public async Task<bool> EliminarUsuario(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> ReactivarUsuario(int id)
        {
           
            var content = new StringContent("", Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/{id}/reactivar", content);
            return response.IsSuccessStatusCode;
        }

        // 7. SUBIR FOTO DE PERFIL
        public async Task<bool> SubirFotoPerfil(int idUsuario, IFormFile archivo)
        {
            using (var content = new MultipartFormDataContent())
            {
                using (var stream = archivo.OpenReadStream())
                {
                    var fileContent = new StreamContent(stream);

                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(archivo.ContentType);

                    
                    content.Add(fileContent, "archivo", archivo.FileName);

                    var response = await _httpClient.PostAsync($"{_baseUrl}/{idUsuario}/foto", content);

                    return response.IsSuccessStatusCode;
                }
            }
        }
    }
}