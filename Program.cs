using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// --- 1. REGISTRO DE SERVICIOS ---

// Configuración de la Base de Datos SQLite
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "mi_base_de_datos.db");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));        

// Configuración de Swagger para documentar la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "Mi API de Visitas", 
        Version = "v1",
        Description = "Una API completa con CRUD y base de datos SQLite"
    });
});

var app = builder.Build();

// --- 2. CONFIGURACIÓN DEL PIPELINE (Middleware) ---

// Habilitar archivos estáticos (para que funcione el index.html en wwwroot)
app.UseDefaultFiles();
app.UseStaticFiles();

// Configurar Swagger en modo desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --- 3. ENDPOINTS (Lógica de la API) ---

// GET: Saludo personalizado
app.MapGet("/hola", (string? nombre) => 
{
    var user = nombre ?? "Visitante";
    return Results.Ok(new { mensaje = $"¡Hola, {user}!", fecha = DateTime.Now });
});

// GET: Listar todas las visitas (ordenadas por fecha)
app.MapGet("/visitas", async (AppDbContext db) => 
    await db.Visitas.OrderByDescending(v => v.FechaRegistro).ToListAsync());

// GET: Buscar visitas por nombre
app.MapGet("/visitas/buscar", async (string termino, AppDbContext db) => 
{
    var resultados = await db.Visitas
        .Where(v => v.Nombre.ToLower().Contains(termino.ToLower()))
        .ToListAsync();
    return Results.Ok(resultados);
});

// GET: Obtener una sola visita por ID
app.MapGet("/visitas/{id:int}", async (int id, AppDbContext db) => 
    await db.Visitas.FindAsync(id) is Visita v ? Results.Ok(v) : Results.NotFound());

// POST: Registrar una nueva visita
app.MapPost("/visitas", async (string nombre, AppDbContext db) => 
{
    if (string.IsNullOrWhiteSpace(nombre)) return Results.BadRequest("El nombre no puede estar vacío.");
    
    var nuevaVisita = new Visita { Nombre = nombre };
    db.Visitas.Add(nuevaVisita);
    await db.SaveChangesAsync();
    
    return Results.Created($"/visitas/{nuevaVisita.Id}", nuevaVisita);
});

// PUT: Actualizar nombre de una visita
app.MapPut("/visitas/{id:int}", async (int id, string nombre, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(nombre)) return Results.BadRequest("El nombre no puede estar vacío.");

    var visita = await db.Visitas.FindAsync(id);
    if (visita is null) return Results.NotFound();

    visita.Nombre = nombre.Trim();
    await db.SaveChangesAsync();

    return Results.Ok(visita);
});

// DELETE: Eliminar una visita
app.MapDelete("/visitas/{id:int}", async (int id, AppDbContext db) => 
{
    var visita = await db.Visitas.FindAsync(id);
    if (visita is null) return Results.NotFound();

    db.Visitas.Remove(visita);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// --- 4. MIGRACIÓN AUTOMÁTICA ---
// (Opcional: Crea la DB al iniciar si no existe)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();