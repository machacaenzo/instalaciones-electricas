using System.IO;
using System.Windows.Media.Imaging;

namespace WPF_InstalacionesElectricas.Utilities
{
    public static class ImagenHelper
    {
        public static BitmapImage Convertir(byte[] arrayBytes)
        {
            if (arrayBytes == null || arrayBytes.Length == 0) return null;

            try
            {
                using (var stream = new MemoryStream(arrayBytes))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    image.Freeze(); // Vital para que no de error de hilos
                    return image;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}