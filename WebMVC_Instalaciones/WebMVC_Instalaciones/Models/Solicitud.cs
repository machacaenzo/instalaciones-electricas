using System.ComponentModel.DataAnnotations;

namespace WebMVC_Instalaciones.Models
{
    public class Solicitud
    {
        public int Id { get; set; }

        [Display(Name = "Dirección del Trabajo")]
        [Required(ErrorMessage = "La dirección es obligatoria")]
        public string Direccion { get; set; }

        [Display(Name = "Descripción del Problema")]
        [Required(ErrorMessage = "Describí brevemente qué necesitás")]
        public string Descripcion { get; set; }

        [Display(Name = "Fecha Deseada")]
        [Required(ErrorMessage = "Seleccioná una fecha")]
        [DataType(DataType.Date)]
        public DateTime FechaDeseada { get; set; }

        public DateTime FechaSolicitud { get; set; } // Fecha en que se pidió

        // Datos de Relación (Vienen de la API)
        public int IdUsuario { get; set; } // El Cliente
        public Usuario? Cliente { get; set; }

        public int IdEstadoSolicitud { get; set; }
        public EstadoSolicitud? Estado { get; set; }

        public bool Activo { get; set; }

        public IFormFile? ArchivoFoto { get; set; }
        public List<Asignacion>? Asignaciones { get; set; }
    }
}