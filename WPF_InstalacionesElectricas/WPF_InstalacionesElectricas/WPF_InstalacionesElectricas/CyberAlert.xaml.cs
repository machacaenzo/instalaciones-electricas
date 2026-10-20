using System;
using System.Windows;
using System.Windows.Media;

namespace WPF_InstalacionesElectricas
{
    // Enum simple
    public enum AlertType { Success, Error, Warning, Info, Confirmation }

    public partial class CyberAlert : Window
    {
        
        private CyberAlert(string titulo, string mensaje, AlertType tipo)
        {
            InitializeComponent();
            ConfigurarAlerta(titulo, mensaje, tipo);
        }

        
        public static bool? Show(string titulo, string mensaje, AlertType tipo)
        {
            
            CyberAlert alerta = new CyberAlert(titulo, mensaje, tipo);
            return alerta.ShowDialog();
        }

        private void ConfigurarAlerta(string titulo, string mensaje, AlertType tipo)
        {
            txtTitulo.Text = titulo;
            txtMensaje.Text = mensaje;

            
            SolidColorBrush colorBrush = (SolidColorBrush)this.FindResource("ColorInfo"); // Default

            // Ocultar todo primero
            IconSuccess.Visibility = Visibility.Collapsed;
            IconError.Visibility = Visibility.Collapsed;
            IconWarning.Visibility = Visibility.Collapsed;
            btnCancelar.Visibility = Visibility.Collapsed;

            switch (tipo)
            {
                case AlertType.Success:
                    colorBrush = (SolidColorBrush)this.FindResource("ColorSuccess");
                    IconSuccess.Visibility = Visibility.Visible;
                    btnAceptar.Content = "CONTINUAR";
                    break;

                case AlertType.Error:
                    colorBrush = (SolidColorBrush)this.FindResource("ColorError");
                    IconError.Visibility = Visibility.Visible;
                    btnAceptar.Content = "ENTENDIDO";
                    break;

                case AlertType.Warning:
                    colorBrush = (SolidColorBrush)this.FindResource("ColorWarning");
                    IconWarning.Visibility = Visibility.Visible;
                    btnAceptar.Content = "ACEPTAR";
                    break;

                case AlertType.Confirmation:
                    colorBrush = (SolidColorBrush)this.FindResource("ColorWarning");
                    IconWarning.Text = "?";
                    IconWarning.Visibility = Visibility.Visible;
                    btnAceptar.Content = "SÍ, CONFIRMAR";
                    btnCancelar.Visibility = Visibility.Visible;
                    break;
            }

            // Aplicar el color seleccionado a los elementos
            IconCircle.BorderBrush = colorBrush;
            MainBorder.BorderBrush = colorBrush;
            GlowEffect.Color = colorBrush.Color;

            // Cambiar el fondo del botón (El estilo AlertButtonStyle usará este color para la sombra también)
            btnAceptar.Background = colorBrush;
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}