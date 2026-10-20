using System.Security.Cryptography;
using System.Text;

namespace WebAPI_InstalacionesElectricas.Utilidades
{
    public static class Encriptador
    {
        public static string EncriptarClave(string clave)
        {
            if (string.IsNullOrEmpty(clave)) return "";

            using (SHA256 sha256 = SHA256.Create())
            {
                // Convertir la cadena a bytes
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(clave));

                // Convertir los bytes a string hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}