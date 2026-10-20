using System.ComponentModel.DataAnnotations;

namespace WebMVC_Instalaciones.Models
{
    public class UsuarioLogin
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")] // 
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Clave { get; set; }
    }
}