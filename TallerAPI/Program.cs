using Microsoft.EntityFrameworkCore;
using TallerAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext
builder.Services.AddDbContext<TallerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("https://taller-frontend.vercel.app", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Habilitar Swagger en la raíz correctamente
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Taller API v1"); // Corrección de la ruta con /swagger inicial
    c.RoutePrefix = string.Empty; // Carga SwaggerUI directamente en la raíz
});

app.UseCors("FrontendPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
