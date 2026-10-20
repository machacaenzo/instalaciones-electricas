namespace WebAPI_InstalacionesElectricas.Models // O DTOs
{
    public class UsuarioSinClave
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string? Celular { get; set; }
        public int IdRol { get; set; }
        public string? NombreRol { get; set; } // Extra: Para no tener que buscar el rol aparte
        public bool Activo { get; set; }

        public byte[]? FotoPerfil { get; set; }
        public string? FotoPerfilTipo { get; set; }
    }
}