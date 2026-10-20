using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAPI_InstalacionesElectricas.Models
{
    public class Rol
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [StringLength(50, ErrorMessage = "El rol no puede exceder los 50 caracteres.")]
        public string Nombre { get; set; } // Ej: Administrador, Cliente, Instalador

        // Relación: Un rol lo tienen muchos usuarios
        [JsonIgnore]
        public ICollection<Usuario>? Usuarios { get; set; }
    }
}