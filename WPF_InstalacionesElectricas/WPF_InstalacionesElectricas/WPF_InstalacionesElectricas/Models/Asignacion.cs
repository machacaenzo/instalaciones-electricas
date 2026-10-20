using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_InstalacionesElectricas.Models
{
    public class Asignacion
    {
        public int Id { get; set; }
                
        public int IdInstalador { get; set; }

        public Usuario Instalador { get; set; }

        public int IdSolicitud { get; set; }

        public Solicitud Solicitud { get; set; }
                
        public DateTime FechaAsignacion { get; set; } = DateTime.Now;
    }
}
