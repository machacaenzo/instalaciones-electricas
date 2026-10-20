using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using WebAPI_InstalacionesElectricas.Models;

namespace WebAPI_InstalacionesElectricas.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Mapeo de las tablas en la base de datos
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<EstadoSolicitud> EstadosSolicitudes { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }
        public DbSet<Asignacion> Asignaciones { get; set; }

        // Configuración profesional de relaciones (Fluent API)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. CONFIGURACIÓN DE USUARIO
            // El correo debe ser ÚNICO en toda la base de datos (Login seguro)
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Correo)
                .IsUnique();

            // Configurar la relación con Rol y el borrado Restringido
            // (No podés borrar un Rol si tiene usuarios asignados)
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.IdRol)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. CONFIGURACIÓN DE ASIGNACIÓN
            // Evitar ciclos de borrado en cascada (lo que pusimos antes)
            modelBuilder.Entity<Asignacion>()
                .HasOne(a => a.Instalador)
                .WithMany(u => u.AsignacionesInstalador)
                .HasForeignKey(a => a.IdInstalador)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}