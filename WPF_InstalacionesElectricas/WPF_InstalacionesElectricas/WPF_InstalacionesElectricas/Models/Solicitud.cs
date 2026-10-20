using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using WPF_InstalacionesElectricas.Utilities;

namespace WPF_InstalacionesElectricas.Models
{
    public class Solicitud
    {
        public int Id { get; set; }
        public string Direccion { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaDeseada { get; set; }
        public bool Activo { get; set; }

        // --- CORRECCIÓN: Usamos los objetos completos ---

        // Antes tenías "int IdEstadoSolicitud", la API manda el objeto "Estado"
        public EstadoSolicitud Estado { get; set; }

        // Antes tenías "int IdUsuario", la API manda el objeto "Cliente"
        public Usuario Cliente { get; set; }
        public byte[] FotoEvidencia { get; set; }
        [JsonIgnore]
        public BitmapImage FotoEvidenciaBitmap => ImagenHelper.Convertir(FotoEvidencia);
    }
}
