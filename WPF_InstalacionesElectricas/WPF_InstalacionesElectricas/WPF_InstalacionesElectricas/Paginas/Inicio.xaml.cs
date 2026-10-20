using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPF_InstalacionesElectricas.Services;
using WPF_InstalacionesElectricas.Utilities;

namespace WPF_InstalacionesElectricas.Paginas
{
    public partial class Inicio : Page
    {
        private readonly SolicitudService _service;

        public Inicio()
        {
            InitializeComponent();
            _service = new SolicitudService();
            CargarDatos();
        }

        private async void CargarDatos()
        {
            var usuario = UsuarioLogueado.ObtenerUsuario();
            if (usuario == null) return;

            txtSaludo.Text = $"HOLA, {usuario.NombreCompleto.ToUpper()}";

            try
            {
                var lista = await _service.ObtenerPorInstalador(usuario.Id);

                // 2. Calcular Contadores
                int pendientes = lista.Count(x => x.Estado.Descripcion == "Pendiente" || x.Estado.Descripcion == "Asignada");
                int proceso = lista.Count(x => x.Estado.Descripcion == "En Proceso");
                int finalizadas = lista.Count(x => x.Estado.Descripcion == "Finalizada" || x.Estado.Descripcion == "Realizada");
                int total = lista.Count;

                // 3. Asignar a la UI
                lblPendientes.Text = pendientes.ToString();
                lblProceso.Text = proceso.ToString();
                lblFinalizadas.Text = finalizadas.ToString();

                // 4. Calcular Porcentaje (Evitar división por cero)
                if (total > 0)
                {
                    double porcentaje = (double)finalizadas / total * 100;
                    pbRendimiento.Value = porcentaje;
                    lblPorcentaje.Text = $"{porcentaje:F0}%";
                }
                else
                {
                    pbRendimiento.Value = 0;
                    lblPorcentaje.Text = "0%";
                }

                // 5. Buscar la "Prioridad"
                var urgente = lista.FirstOrDefault(x => x.Estado.Descripcion == "En Proceso");

                if (urgente == null) 
                {
                    urgente = lista
                        .Where(x => x.Estado.Descripcion == "Pendiente" || x.Estado.Descripcion == "Asignada")
                        .OrderBy(x => x.FechaDeseada) // La fecha más vieja primero
                        .FirstOrDefault();
                }

                if (urgente != null)
                {
                    txtUrgenteDireccion.Text = urgente.Direccion;
                    txtUrgenteFecha.Text = "Programada para: " + urgente.FechaDeseada.ToShortDateString();
                }
                else
                {
                    // Si no hay nada de nada
                    txtUrgenteDireccion.Text = "Sin tareas pendientes";
                    txtUrgenteFecha.Text = "¡Excelente trabajo!";
                }

            }
            catch (Exception)
            {
                lblPendientes.Text = "-";
            }
        }

        private void btnIrTablero_Click(object sender, RoutedEventArgs e)
        {
            // Navegar a la página de Tareas desde el Frame principal
            this.NavigationService.Navigate(new Tareas());
        }
    }
}