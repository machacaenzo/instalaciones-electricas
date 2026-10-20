using System.Windows;
using System.Windows.Input;
using WPF_InstalacionesElectricas.Models;

namespace WPF_InstalacionesElectricas.Paginas
{
    public partial class DetalleSolicitud : Window
    {
        public DetalleSolicitud(Solicitud solicitud)
        {
            InitializeComponent();

        
            this.DataContext = solicitud;

            if (solicitud.FotoEvidenciaBitmap == null)
            {
                imgEvidencia.Visibility = Visibility.Collapsed;
                pnlNoFoto.Visibility = Visibility.Visible;
            }
            else
            {
                imgEvidencia.Visibility = Visibility.Visible;
                pnlNoFoto.Visibility = Visibility.Collapsed;
            }

            this.MouseLeftButtonDown += (s, e) => this.DragMove();
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}