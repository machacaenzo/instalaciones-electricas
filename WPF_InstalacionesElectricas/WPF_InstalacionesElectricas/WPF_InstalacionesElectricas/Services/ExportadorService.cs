using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using WPF_InstalacionesElectricas.Models;

namespace WPF_InstalacionesElectricas.Services
{
    public static class ExportadorService
    {
        
        // EXPORTAR A EXCEL (.xlsx) - USANDO CLOSEDXML
        
        public static void ExportarExcel(List<Solicitud> datos, string ruta)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte");

                // 1. CABECERAS (Con estilo)
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "FECHA";
                worksheet.Cell(1, 3).Value = "CLIENTE";
                worksheet.Cell(1, 4).Value = "DIRECCIÓN";
                worksheet.Cell(1, 5).Value = "ESTADO";
                worksheet.Cell(1, 6).Value = "DESCRIPCIÓN";

                // Poner negrita y fondo gris a la cabecera
                var rangoHeader = worksheet.Range("A1:F1");
                rangoHeader.Style.Font.Bold = true;
                rangoHeader.Style.Fill.BackgroundColor = XLColor.LightGray;

                // 2. LLENAR DATOS
                int fila = 2;
                foreach (var item in datos)
                {
                    worksheet.Cell(fila, 1).Value = item.Id;
                    worksheet.Cell(fila, 2).Value = item.FechaDeseada; // Excel detecta que es fecha solo
                    worksheet.Cell(fila, 3).Value = item.Cliente.NombreCompleto;
                    worksheet.Cell(fila, 4).Value = item.Direccion;
                    worksheet.Cell(fila, 5).Value = item.Estado.Descripcion;
                    worksheet.Cell(fila, 6).Value = item.Descripcion;

                    // Colorear celda según estado (Opcional, queda muy pro)
                    if (item.Estado.Descripcion == "Finalizada")
                        worksheet.Cell(fila, 5).Style.Font.FontColor = XLColor.Green;
                    else if (item.Estado.Descripcion == "En Proceso")
                        worksheet.Cell(fila, 5).Style.Font.FontColor = XLColor.Blue;

                    fila++;
                }

                // 3. AUTOAJUSTAR COLUMNAS
                worksheet.Columns().AdjustToContents();

                // 4. GUARDAR
                workbook.SaveAs(ruta);
            }
        }

        
        // EXPORTAR A PDF - USANDO ITEXTSHARP
        public static void ExportarPdf(List<Solicitud> datos, string ruta)
        {
            // 1. Configurar documento (A4 Horizontal para que entre la tabla)
            Document doc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);

            try
            {
                PdfWriter.GetInstance(doc, new FileStream(ruta, FileMode.Create));
                doc.Open();

                // 2. Título
                var fuenteTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                doc.Add(new Paragraph("REPORTE DE INSTALACIONES", fuenteTitulo));
                doc.Add(new Paragraph($"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}"));
                doc.Add(new Paragraph(" ")); // Espacio

                // 3. Crear Tabla (5 Columnas)
                PdfPTable tabla = new PdfPTable(5);
                tabla.WidthPercentage = 100;
                // Anchos relativos de columnas
                tabla.SetWidths(new float[] { 1f, 2f, 3f, 3f, 2f });

                // 4. Cabeceras
                string[] columnas = { "ID", "FECHA", "CLIENTE", "DIRECCIÓN", "ESTADO" };
                foreach (var col in columnas)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(col, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE)));
                    cell.BackgroundColor = BaseColor.DARK_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5;
                    tabla.AddCell(cell);
                }

                // 5. Datos
                var fuenteDatos = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                foreach (var item in datos)
                {
                    tabla.AddCell(new Phrase(item.Id.ToString(), fuenteDatos));
                    tabla.AddCell(new Phrase(item.FechaDeseada.ToString("dd/MM/yyyy"), fuenteDatos));
                    tabla.AddCell(new Phrase(item.Cliente.NombreCompleto, fuenteDatos));
                    tabla.AddCell(new Phrase(item.Direccion, fuenteDatos));

                    // Celda estado
                    PdfPCell celdaEstado = new PdfPCell(new Phrase(item.Estado.Descripcion, fuenteDatos));
                    // Colorcito suave de fondo según estado
                    if (item.Estado.Descripcion == "Finalizada") celdaEstado.BackgroundColor = new BaseColor(200, 255, 200); // Verde claro
                    else if (item.Estado.Descripcion == "En Proceso") celdaEstado.BackgroundColor = new BaseColor(200, 240, 255); // Azul claro

                    tabla.AddCell(celdaEstado);
                }

                doc.Add(tabla);
            }
            finally
            {
                doc.Close();
            }
        }
    }
}