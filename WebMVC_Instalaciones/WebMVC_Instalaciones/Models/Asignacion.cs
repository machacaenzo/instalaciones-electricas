namespace WebMVC_Instalaciones.Models
{
    public class Asignacion
    {
        public int Id { get; set; }
        public int IdInstalador { get; set; }
        public int IdSolicitud { get; set; }

        // Propiedad de navegación para ver el nombre del técnico
        public Usuario? Instalador { get; set; }
    }
}