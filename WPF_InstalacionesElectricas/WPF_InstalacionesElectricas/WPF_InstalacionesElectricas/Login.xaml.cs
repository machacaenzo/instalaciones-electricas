using System;
using System.Windows;
using System.Windows.Input;
using WPF_InstalacionesElectricas.Services;
using WPF_InstalacionesElectricas.Utilities;

namespace WPF_InstalacionesElectricas
{
    public partial class Login : Window
    {
        private readonly AuthService _authService;

        // Textos para el placeholder
        private const string DefaultEmailText = "admin@empresa.com";
        private const string DefaultPasswordText = "password";

        public Login()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        // --- Eventos de UI ---
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
        private void btnCerrar_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        // --- Lógica de Placeholders (Watermark) ---
        private void txtCorreo_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        { if (txtCorreo.Text == DefaultEmailText) txtCorreo.Text = ""; }

        private void txtCorreo_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        { if (string.IsNullOrWhiteSpace(txtCorreo.Text)) txtCorreo.Text = DefaultEmailText; }

        private void txtClave_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        { if (txtClave.Password == DefaultPasswordText) txtClave.Password = ""; }

        private void txtClave_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        { if (string.IsNullOrWhiteSpace(txtClave.Password)) txtClave.Password = DefaultPasswordText; }

        // --- LÓGICA DE LOGIN ---
   
        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            btnLogin.IsEnabled = false;

            string correo = txtCorreo.Text.Trim();
            string clave = txtClave.Password;

            // 1. Validaciones básicas visuales
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(clave) || correo == DefaultEmailText)
            {
                CyberAlert.Show("DATOS INCOMPLETOS", "Por favor, ingrese su usuario y contraseña.", AlertType.Warning);
                btnLogin.IsEnabled = true;
                return;
            }

            try
            {
                // 2. Llamada a la API (LOGIN)
                var usuario = await _authService.Login(correo, clave);

                if (usuario != null)
                {
                    // 3. Validar Rol
                    if (usuario.NombreRol == "Instalador")
                    {
                        
                        // 4. PASO EXTRA: RECUPERAR LA FOTO 
                       
                        try
                        {
                            // Intentamos bajar la foto usando el ID que nos dio el Login
                            byte[] fotoBytes = await ApiClient.GetBytesAsync($"Usuarios/{usuario.Id}/foto");

                            if (fotoBytes != null && fotoBytes.Length > 0)
                            {
                                usuario.FotoPerfil = fotoBytes; 
                            }
                        }
                        catch
                        {
                            
                            Console.WriteLine("No se pudo descargar la foto de perfil.");
                        }
                      

                        // 5. Guardar sesión
                        UsuarioLogueado.IniciarSesion(usuario);

                        // 6. MOSTRAR ALERTA DE BIENVENIDA 
                        CyberAlert.Show("¡HOLA!", $"Hola, {usuario.NombreCompleto}. Acceso autorizado.", AlertType.Success);

                        // 7. Abrir Principal
                        var main = new MainWindow(usuario);
                        main.Show();
                        this.Close();
                    }
                    else
                    {
                        // Error de Rol
                        CyberAlert.Show("ACCESO DENEGADO", $"El rol '{usuario.NombreRol}' no tiene permisos para usar la terminal de campo.", AlertType.Error);
                    }
                }
                else
                {
                    // Error de Credenciales
                    CyberAlert.Show("ERROR DE ACCESO", "Usuario o contraseña incorrectos.", AlertType.Error);
                }
            }
            catch (Exception ex)
            {
                // Error de Conexión
                CyberAlert.Show("ERROR DE CONEXIÓN", "No se pudo conectar con el servidor.\n" + ex.Message, AlertType.Error);
            }
            finally
            {
                btnLogin.IsEnabled = true;
            }
        }
    }
}