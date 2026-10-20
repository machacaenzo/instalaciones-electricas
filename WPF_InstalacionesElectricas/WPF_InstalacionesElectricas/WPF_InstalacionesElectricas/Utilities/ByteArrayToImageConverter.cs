using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WPF_InstalacionesElectricas.Utilities
{
    public class ByteArrayToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Si el valor es nulo o no es un array de bytes, devolvemos null (o una imagen por defecto)
            if (value is byte[] byteArray && byteArray.Length > 0)
            {
                try
                {
                    using (var stream = new MemoryStream(byteArray))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad; // Importante para liberar memoria
                        image.StreamSource = stream;
                        image.EndInit();
                        image.Freeze(); // Importante para rendimiento
                        return image;
                    }
                }
                catch
                {
                    return null; // Si falla la conversión, no mostramos nada
                }
            }
            return null; // O podrías retornar una imagen "placeholder" (sin foto)
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // No necesitamos convertir de imagen a bytes en la vista
        }
    }
}
