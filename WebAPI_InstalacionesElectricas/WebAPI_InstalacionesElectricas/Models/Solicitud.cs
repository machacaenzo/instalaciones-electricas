using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebAPI_InstalacionesElectricas.Models
{
    public class Solicitud
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(200)]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "La descripción del trabajo es obligatoria.")]
        [StringLength(500)]
        public string Descripcion { get; set; } 

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaSolicitud { get; set; } = DateTime.Now; 

        [Required(ErrorMessage = "La fecha deseada es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaDeseada { get; set; }

      

        
        [Required]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario? Cliente { get; set; }

        
        [Required]
        public int IdEstadoSolicitud { get; set; }

        [ForeignKey("IdEstadoSolicitud")]
        public EstadoSolicitud? Estado { get; set; }

       
        public ICollection<Asignacion>? Asignaciones { get; set; }
        public bool Activo { get; set; } = true;

        public byte[]? FotoEvidencia { get; set; }
        public string? FotoEvidenciaTipo { get; set; }
    }
}