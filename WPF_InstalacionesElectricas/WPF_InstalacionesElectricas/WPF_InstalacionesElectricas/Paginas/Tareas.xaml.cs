using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPF_InstalacionesElectricas.Models;
using WPF_InstalacionesElectricas.Services;
using WPF_InstalacionesElectricas.Utilities;

namespace WPF_InstalacionesElectricas.Paginas
{
    public partial class Tareas : Page
    {
        private readonly SolicitudService _solicitudService;

        public Tareas()
        {
            InitializeComponent();
            _solicitudService = new SolicitudService();
            CargarTablero();
        }

        private async void CargarTablero()
        {
            var usuario = UsuarioLogueado.ObtenerUsuario();
            if (usuario == null) return;

            try
            {
                var todasLasTareas = await _solicitudService.ObtenerPorInstalador(usuario.Id);

                
                ListPendientes.ItemsSource = todasLasTareas
                    .Where(t => t.Estado.Descripcion == "Pendiente" || t.Estado.Descripcion == "Asignada")
                    .ToList();

                ListProceso.ItemsSource = todasLasTareas
                    .Where(t => t.Estado.Descripcion == "En Proceso")
                    .ToList();

                ListFinalizadas.ItemsSource = todasLasTareas
                    .Where(t => t.Estado.Descripcion == "Finalizada" || t.Estado.Descripcion == "Realizada")
                    .ToList();
            }
            catch (Exception ex)
            {
                CyberAlert.Show("ERROR DE CONEXIÓN", "No se pudo cargar el tablero.\n" + ex.Message, AlertType.Error);
            }
        }

        // --- COLUMNA 1: INICIAR TAREA ---
        private async void btnIniciar_Click(object sender, RoutedEventArgs e)
        {
            var solicitud = (sender as Button).DataContext as Solicitud;
            if (solicitud == null) return;

            // 1. PREGUNTAR ANTES DE MOVER
            bool? confirmar = CyberAlert.Show(
                "¿INICIAR TRABAJO?",
                $"Vas a comenzar la tarea en:\n{solicitud.Direccion}\n\n¿Confirmar inicio?",
                AlertType.Confirmation
            );

            if (confirmar == true)
            {
                // 2. LLAMAR A LA API 
              
                bool exito = await _solicitudService.ActualizarEstado(solicitud.Id, EstadoID.EnProceso);

                if (exito)
                {
                    // 3. AVISAR QUE SALIÓ BIEN
                    CyberAlert.Show("¡TAREA INICIADA!", "El estado cambió a 'En Proceso'. ¡Buen trabajo!", AlertType.Success);

                    // 4. RECARGAR
                    CargarTablero();
                }
                else
                {
                    CyberAlert.Show("ERROR", "No se pudo conectar con el servidor.", AlertType.Error);
                }
            }
        }

        // --- COLUMNA 2: FINALIZAR TAREA ---
        private async void btnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            var solicitud = (sender as Button).DataContext as Solicitud;
            if (solicitud == null) return;

            // 1. PREGUNTAR ANTES DE CERRAR
            bool? confirmar = CyberAlert.Show(
                "¿FINALIZAR TRABAJO?",
                "¿Confirmas que la instalación está 100% terminada y probada?",
                AlertType.Confirmation
            );

            if (confirmar == true)
            {
                // 2. LLAMAR A LA API 
               
                bool exito = await _solicitudService.ActualizarEstado(solicitud.Id, EstadoID.Finalizada);

                if (exito)
                {
                    // 3. AVISAR QUE SALIÓ BIEN
                    CyberAlert.Show("¡FELICITACIONES!", "Tarea registrada como FINALIZADA exitosamente.", AlertType.Success);

                    // 4. RECARGAR
                    CargarTablero();
                }
                else
                {
                    CyberAlert.Show("ERROR", "No se pudo guardar el cambio.", AlertType.Error);
                }
            }
        }

        // --- VER DETALLES  ---
        private void btnDetalle_Click(object sender, RoutedEventArgs e)
        {
            var solicitud = (sender as Button).DataContext as Solicitud;
            if (solicitud == null) return;

            // Abrimos la ventana de solo lectura con la foto
            DetalleSolicitud ventana = new DetalleSolicitud(solicitud);
            ventana.ShowDialog();

           
        }
    }
}