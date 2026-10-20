using System.Text;
using System.Text.Json;
using WebMVC_Instalaciones.Models;
using System.Net.Http.Headers; 
namespace WebMVC_Instalaciones.Services
{
    public class SolicitudService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public SolicitudService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _baseUrl = "https://localhost:7088/api/Solicitudes";
        }

        // 1. OBTENER HISTORIAL DEL CLIENTE
        public async Task<List<Solicitud>> ObtenerPorCliente(int idCliente)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/cliente/{idCliente}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<Solicitud>>(json, opciones) ?? new List<Solicitud>();
            }
            return new List<Solicitud>();
        }

        // 2. CREAR NUEVA SOLICITUD 
        public async Task<int> CrearSolicitud(Solicitud solicitud)
        {
            var json = JsonSerializer.Serialize(solicitud);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_baseUrl, content);

            if (response.IsSuccessStatusCode)
            {
               
                var jsonRespuesta = await response.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var solicitudCreada = JsonSerializer.Deserialize<Solicitud>(jsonRespuesta, opciones);

                return solicitudCreada?.Id ?? 0;
            }

            return 0; 
        }

        // 3. OBTENER TODAS (Para el Admin)
        public async Task<List<Solicitud>> ObtenerTodas()
        {
            var response = await _httpClient.GetAsync(_baseUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<Solicitud>>(json, opciones) ?? new List<Solicitud>();
            }
            return new List<Solicitud>();
        }

        // 4. OBTENER TRABAJOS DEL INSTALADOR
        public async Task<List<Solicitud>> ObtenerPorInstalador(int idInstalador)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/instalador/{idInstalador}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<Solicitud>>(json, opciones) ?? new List<Solicitud>();
            }
            return new List<Solicitud>();
        }

        // 5. ELIMINAR
        public async Task<bool> EliminarSolicitud(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }

        // 7. OBTENER UNA SOLA POR ID
        public async Task<Solicitud?> ObtenerPorId(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<Solicitud>(json, opciones);
            }
            return null;
        }

        // 8. EDITAR SOLICITUD
        public async Task<bool> EditarSolicitud(Solicitud solicitud)
        {
            var json = JsonSerializer.Serialize(solicitud);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_baseUrl}/{solicitud.Id}", content);

            return response.IsSuccessStatusCode;
        }

        // 9. SUBIR EVIDENCIA 
        public async Task<bool> SubirFotoEvidencia(int idSolicitud, IFormFile archivo)
        {
            using (var content = new MultipartFormDataContent())
            {
                using (var stream = archivo.OpenReadStream())
                {
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(archivo.ContentType);

                   
                    content.Add(fileContent, "archivo", archivo.FileName);

                    var response = await _httpClient.PostAsync($"{_baseUrl}/{idSolicitud}/foto", content);

                    return response.IsSuccessStatusCode;
                }
            }
        }
    }
}