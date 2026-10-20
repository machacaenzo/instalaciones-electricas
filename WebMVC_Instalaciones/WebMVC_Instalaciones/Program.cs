var builder = WebApplication.CreateBuilder(args);

// 1. Agregar servicios al contenedor.
builder.Services.AddControllersWithViews();

// 2. CONFIGURAR HTTP CLIENT (Vital para llamar a tu API)
builder.Services.AddHttpClient();
builder.Services.AddScoped<WebMVC_Instalaciones.Services.AuthService>();
builder.Services.AddScoped<WebMVC_Instalaciones.Services.SolicitudService>();
builder.Services.AddScoped<WebMVC_Instalaciones.Services.UsuarioService>();
builder.Services.AddScoped<WebMVC_Instalaciones.Services.AsignacionService>();
// 3. CONFIGURAR AUTENTICACIÓN (Cookies)
// Esto permite que el sistema recuerde al usuario después del Login
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.Cookie.Name = "SistemaInstalaciones.Cookie";
        options.LoginPath = "/Account/Login";     // Si no está logueado, va al Login
        options.AccessDeniedPath = "/Home/Index"; // Si no tiene permiso, va al Inicio
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Sesión de 1 hora
    });

var app = builder.Build();
Rotativa.AspNetCore.RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // En producción usamos una vista de error genérica
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();



// 4. ACTIVAR LA SEGURIDAD (El orden es sagrado: Auth -> Author)
app.UseAuthentication(); // ¿Quién sos?
app.UseAuthorization();  // ¿Qué podés hacer?

// 5. RUTA POR DEFECTO
// Arranca en el Home (Página informativa)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();