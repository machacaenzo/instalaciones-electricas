using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebAPI_InstalacionesElectricas.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        [StringLength(100)]
        public string Correo { get; set; }

        
        [StringLength(255)] // Longitud alta por si decidimos encriptarla a futuro
        public string? Clave { get; set; }

        [StringLength(20, ErrorMessage = "El celular no puede exceder los 20 caracteres.")]
        public string? Celular { get; set; }

        // --- Claves Foráneas ---
        [Required]
        public int IdRol { get; set; }

        [ForeignKey("IdRol")]
        public Rol? Rol { get; set; }

        // --- Propiedades de Navegación (Listas) ---

        // Solicitudes que hizo este usuario (si es Cliente)
        [JsonIgnore]
        public ICollection<Solicitud>? SolicitudesCliente { get; set; }

        // Asignaciones que tiene este usuario (si es Instalador)
        [JsonIgnore]
        public ICollection<Asignacion>? AsignacionesInstalador { get; set; }

        public bool Activo { get; set; } = true; // Por defecto nace activo


        public byte[]? FotoPerfil { get; set; }
        public string? FotoPerfilTipo { get; set; }
    }
}