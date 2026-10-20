using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WPF_InstalacionesElectricas.Utilities;

namespace WPF_InstalacionesElectricas.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string Celular { get; set; }
        
        public string NombreRol { get; set; }
        public byte[] FotoPerfil { get; set; }
        [JsonIgnore] // Le decimos a la API que ignore esto
        public BitmapImage FotoPerfilBitmap => ImagenHelper.Convertir(FotoPerfil);
    }
}