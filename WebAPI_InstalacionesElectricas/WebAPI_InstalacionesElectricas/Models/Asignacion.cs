using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI_InstalacionesElectricas.Models
{
    public class Asignacion
    {
        [Key]
        public int Id { get; set; }

        // A qué técnico se le asignó
        [Required]
        public int IdInstalador { get; set; }

        [ForeignKey("IdInstalador")]
        public Usuario? Instalador { get; set; }

        // Qué solicitud tiene que atender
        [Required]
        public int IdSolicitud { get; set; }

        [ForeignKey("IdSolicitud")]
        public Solicitud? Solicitud { get; set; }

        // Fecha en la que el admin hizo la asignación
        public DateTime FechaAsignacion { get; set; } = DateTime.Now;
    }
}