using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WPF_InstalacionesElectricas.Services
{
  
    public static class ApiClient
    {

        private const string BaseUrl = "https://localhost:7088/api/";

        private static readonly HttpClient _client;

        static ApiClient()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(BaseUrl);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
        }

        // --- MÉTODO GENÉRICO PARA OBTENER DATOS (GET) ---
      
        public static async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                // Hacemos la llamada asíncrona
                HttpResponseMessage response = await _client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }

                return default(T);
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error GET en {endpoint}: {ex.Message}");
                return default(T);
            }
        }

        // --- MÉTODO GENÉRICO PARA ENVIAR DATOS (POST) ---
       
        public static async Task<T> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseString);
                }

                return default(T);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error POST en {endpoint}: {ex.Message}");
                throw; 
            }
        }

        // --- MÉTODO PARA ACTUALIZAR (PUT) ---
        
        public static async Task<bool> PutAsync(string endpoint, object data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PutAsync(endpoint, content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error PUT en {endpoint}: {ex.Message}");
                return false;
            }
        }
        // --- MÉTODO PARA DESCARGAR ARCHIVOS (FOTOS) ---
        public static async Task<byte[]> GetBytesAsync(string endpoint)
        {
            try
            {
                // Pide el archivo al servidor
                HttpResponseMessage response = await _client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    // Devuelve el array de bytes (la imagen cruda)
                    return await response.Content.ReadAsByteArrayAsync();
                }
                return null; // Si no hay foto o falla
            }
            catch
            {
                return null;
            }
        }
    }
}