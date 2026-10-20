using System.Text;
using System.Text.Json;

namespace WebMVC_Instalaciones.Services
{
    public class AsignacionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public AsignacionService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
           
            _baseUrl = "https://localhost:7088/api/Asignaciones";
        }

        public async Task<bool> AsignarTecnico(int idSolicitud, int idInstalador)
        {
            var datos = new
            {
                IdSolicitud = idSolicitud,
                IdInstalador = idInstalador
            };

            var json = JsonSerializer.Serialize(datos);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_baseUrl, content);

            
            return response.IsSuccessStatusCode;
        }
    }
}