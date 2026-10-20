using System;
using System.Collections.Generic;
using System.IO; 
using System.Linq;
using System.Text; 
using System.Windows;
using System.Windows.Controls;
using WPF_InstalacionesElectricas.Models;
using WPF_InstalacionesElectricas.Services;
using WPF_InstalacionesElectricas.Utilities;

namespace WPF_InstalacionesElectricas.Paginas
{
    public partial class Reportes : Page
    {
        private readonly SolicitudService _service;
        private List<Solicitud> _listaCompleta; 

        public Reportes()
        {
            InitializeComponent();
            _service = new SolicitudService();

            // Configuramos fechas por defecto (Este mes)
            dpDesde.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dpHasta.SelectedDate = DateTime.Now.AddMonths(1); 

            CargarDatosIniciales();
        }

        private async void CargarDatosIniciales()
        {
            var usuario = UsuarioLogueado.ObtenerUsuario();
            if (usuario == null) return;

            try
            {
                _listaCompleta = await _service.ObtenerPorInstalador(usuario.Id);

                Filtrar();
            }
            catch (Exception ex)
            {
                CyberAlert.Show("ERROR", "No se pudieron cargar los datos.\n" + ex.Message, AlertType.Error);
            }
        }

        private void btnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            Filtrar();
        }

        private void Filtrar()
        {
            
            if (_listaCompleta == null) return;

            var filtrada = _listaCompleta.AsEnumerable();

            // 1. FILTRO FECHA DESDE
            if (dpDesde.SelectedDate.HasValue)
                filtrada = filtrada.Where(x => x.FechaDeseada.Date >= dpDesde.SelectedDate.Value.Date);

            // 2. FILTRO FECHA HASTA
            if (dpHasta.SelectedDate.HasValue)
                filtrada = filtrada.Where(x => x.FechaDeseada.Date <= dpHasta.SelectedDate.Value.Date);

            // 3. NUEVO: FILTRO POR ESTADO
            
            if (cbEstado.SelectedItem is ComboBoxItem itemSeleccionado)
            {
                string estado = itemSeleccionado.Content.ToString();

              
                if (estado != "Todos")
                {
                    filtrada = filtrada.Where(x => x.Estado.Descripcion == estado);
                }
            }

            // Ordenar y mostrar
            var resultado = filtrada.OrderByDescending(x => x.FechaDeseada).ToList();

            dgReportes.ItemsSource = resultado;
            txtTotal.Text = $"{resultado.Count} registros encontrados";
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            
            dpDesde.SelectedDate = null;
            dpHasta.SelectedDate = null;

            cbEstado.SelectedIndex = 0;

            Filtrar();
        }
       

        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            var datos = dgReportes.ItemsSource as List<Solicitud>;
            if (datos == null || datos.Count == 0) { CyberAlert.Show("VACÍO", "No hay datos para exportar.", AlertType.Warning); return; }

            try
            {
                // Guardamos en Documentos con extensión .xlsx
                string ruta = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                              $"Reporte_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");

                ExportadorService.ExportarExcel(datos, ruta);

                if (CyberAlert.Show("ÉXITO", "Excel generado correctamente.\n¿Abrir archivo?", AlertType.Success) == true)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(ruta) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                CyberAlert.Show("ERROR", ex.Message, AlertType.Error);
            }
        }

        private void btnPdf_Click(object sender, RoutedEventArgs e)
        {
            var datos = dgReportes.ItemsSource as List<Solicitud>;
            if (datos == null || datos.Count == 0) { CyberAlert.Show("VACÍO", "No hay datos para exportar.", AlertType.Warning); return; }

            try
            {
                // Guardamos en Documentos con extensión .pdf
                string ruta = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                              $"Reporte_{DateTime.Now:yyyyMMdd_HHmm}.pdf");

                ExportadorService.ExportarPdf(datos, ruta);

                if (CyberAlert.Show("ÉXITO", "PDF generado correctamente.\n¿Abrir archivo?", AlertType.Success) == true)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(ruta) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                CyberAlert.Show("ERROR", ex.Message, AlertType.Error);
            }
        }
    }
}