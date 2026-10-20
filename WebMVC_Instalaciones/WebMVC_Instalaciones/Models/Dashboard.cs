namespace WebMVC_Instalaciones.Models
{
    public class Dashboard
    {
        // Tarjetas Principales
        public int CantidadPendientes { get; set; }
        public int CantidadEnProceso { get; set; }
        public int CantidadFinalizadas { get; set; }

        // Usuarios
        public int TotalUsuarios { get; set; }
        public int TotalTecnicos { get; set; }

        // Para los porcentajes (Barras de progreso)
        public int PorcentajePendientes { get; set; }
        public int PorcentajeFinalizadas { get; set; }

        // Lista de las últimas 5 (Para mostrar actividad reciente)
        public List<Solicitud> UltimasSolicitudes { get; set; } = new List<Solicitud>();
    }
}