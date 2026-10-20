using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAPI_InstalacionesElectricas.Models
{
    public class EstadoSolicitud
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La descripción del estado es obligatoria.")]
        [StringLength(50)]
        public string Descripcion { get; set; } // Ej: Pendiente, Realizada

        // Relación: Un estado puede estar en muchas solicitudes
        [JsonIgnore]
        public ICollection<Solicitud>? Solicitudes { get; set; }
    }
}