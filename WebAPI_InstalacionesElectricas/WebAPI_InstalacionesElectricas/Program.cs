using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebAPI_InstalacionesElectricas.Data;
using WebAPI_InstalacionesElectricas.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Configuración de la Base de Datos (SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Inyección de Dependencias (Servicios)
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<SolicitudService>();
builder.Services.AddScoped<AsignacionService>();
builder.Services.AddScoped<RolService>();
builder.Services.AddScoped<EstadoSolicitudService>();

// 3. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 4. CONTROLADORES CON CONFIGURACIÓN JSON 
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// 5 REGISTRAR EL WORKER
builder.Services.AddHostedService<WebAPI_InstalacionesElectricas.Workers.CanceladorAutomatico>();
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Habilitar Archivos Estáticos (Para que se puedan ver las fotos subidas)
app.UseStaticFiles();

app.UseCors("PermitirTodo");
app.UseAuthorization();

app.MapControllers();

// BLOQUE DE INICIALIZACIÓN DE DATOS
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al sembrar la base de datos.");
    }
}

app.Run();