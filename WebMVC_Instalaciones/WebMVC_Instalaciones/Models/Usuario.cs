using Microsoft.AspNetCore.Http;
namespace WebMVC_Instalaciones.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string? Celular { get; set; }
        public int IdRol { get; set; }
        public string NombreRol { get; set; } // "Cliente", "Administrador"
        public bool Activo { get; set; }

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string? Clave { get; set; }


        public IFormFile? ArchivoFoto { get; set; }
    }
}