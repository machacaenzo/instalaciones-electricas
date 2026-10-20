using System;
using System.Windows;
using System.Windows.Input;
using WPF_InstalacionesElectricas.Paginas; 
using WPF_InstalacionesElectricas.Models;
using WPF_InstalacionesElectricas.Utilities;

namespace WPF_InstalacionesElectricas
{
    public partial class MainWindow : Window
    {
        private Usuario _usuarioActual;

        public MainWindow(Usuario usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;

            // 1. Asignar contexto para la foto de perfil del menú lateral
            this.DataContext = _usuarioActual;

            // 2. Suscribirse al evento de maximizar
            this.StateChanged += MainWindow_StateChanged;

            
            WelcomePanel.Visibility = Visibility.Collapsed;
            MainFrame.Navigate(new Paginas.Inicio());
        }

        // --- LÓGICA DE VENTANA 
        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
               
                RootGrid.Margin = new Thickness(7);
                BackgroundBorder.CornerRadius = new CornerRadius(0);
            }
            else
            {
               
                RootGrid.Margin = new Thickness(0);
                BackgroundBorder.CornerRadius = new CornerRadius(18);
            }
        }

        // --- NAVEGACIÓN ---

        // BOTÓN INICIO
        private void btnPrincipal_Click(object sender, RoutedEventArgs e)
        {
            
            WelcomePanel.Visibility = Visibility.Collapsed;
            MainFrame.Navigate(new Paginas.Inicio());
        }

        // BOTÓN TAREAS 
        private void btnTareas_Click(object sender, RoutedEventArgs e)
        {
            WelcomePanel.Visibility = Visibility.Collapsed;
            MainFrame.Navigate(new Tareas());
        }
        private void btnReportes_Click(object sender, RoutedEventArgs e)
        {
            WelcomePanel.Visibility = Visibility.Collapsed;
            MainFrame.Navigate(new Paginas.Reportes());
        }

        // --- CONTROL DE VENTANA 
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btnMinimizar_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;

        private void btnMaximizar_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                
                this.WindowState = WindowState.Normal;
            }
            else
            {
                
                this.WindowState = WindowState.Maximized;
            }
        }

        private void btnSalirApp_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        // --- CERRAR SESIÓN ---
        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            UsuarioLogueado.CerrarSesion();
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }
    }
}