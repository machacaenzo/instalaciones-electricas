using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_InstalacionesElectricas.Models;

namespace WPF_InstalacionesElectricas.Utilities
{
    public static class UsuarioLogueado
    {
        private static Usuario _usuario;

        // Guardamos el objeto entero (ID, Nombre, Rol, Token)
        public static void IniciarSesion(Usuario usuario)
        {
            _usuario = usuario;
        }

        public static void CerrarSesion()
        {
            _usuario = null;
        }

        public static Usuario ObtenerUsuario()
        {
            return _usuario;
        }

        // Helper rápido para ver si hay alguien
        public static bool EstaAutenticado()
        {
            return _usuario != null;
        }
    }
}
