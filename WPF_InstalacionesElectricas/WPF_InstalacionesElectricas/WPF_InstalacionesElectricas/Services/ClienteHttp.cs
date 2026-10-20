using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WPF_InstalacionesElectricas.Services
{
    public static class ClienteHttp
    {
        // CAMBIÁ EL PUERTO SI ES NECESARIO
        private const string BaseUrl = "https://localhost:7088/api/";

        public static HttpClient ObtenerCliente()
        {
            // Parche para SSL en localhost
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}