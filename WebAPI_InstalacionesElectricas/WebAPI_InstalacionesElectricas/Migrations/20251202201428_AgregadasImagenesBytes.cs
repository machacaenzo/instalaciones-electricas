using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI_InstalacionesElectricas.Migrations
{
    /// <inheritdoc />
    public partial class AgregadasImagenesBytes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Clave",
                table: "Usuarios",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<byte[]>(
                name: "FotoPerfil",
                table: "Usuarios",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoPerfilTipo",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FotoEvidencia",
                table: "Solicitudes",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoEvidenciaTipo",
                table: "Solicitudes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoPerfil",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FotoPerfilTipo",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FotoEvidencia",
                table: "Solicitudes");

            migrationBuilder.DropColumn(
                name: "FotoEvidenciaTipo",
                table: "Solicitudes");

            migrationBuilder.AlterColumn<string>(
                name: "Clave",
                table: "Usuarios",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
