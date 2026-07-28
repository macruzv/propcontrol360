using Microsoft.EntityFrameworkCore;
using propcontrol360.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container con compilación de vistas Razor en tiempo de ejecución
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// Configurar SQLite con EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=propcontrol360.db"));

var app = builder.Build();

// Inicializar y sembrar la base de datos de manera automática
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
        logger.LogError(ex, "Ocurrió un error al inicializar la base de datos.");
    }
}

// Configuración del Pipeline HTTP (Igual que en MC-Solutions)
app.UseDeveloperExceptionPage();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
