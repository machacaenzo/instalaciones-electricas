using System.ComponentModel.DataAnnotations;

namespace WebMVC_Instalaciones.Models
{
    public class UsuarioRegistro
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public string Clave { get; set; }

        [Compare("Clave", ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        public string ConfirmarClave { get; set; }

        public string? Celular { get; set; }
        public int IdRol { get; set; }
    }
}