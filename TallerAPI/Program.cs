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

// Habilitar Swagger siempre (producción y desarrollo)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("v1/swagger.json", "Taller API v1"); // Ruta relativa corregida
    c.RoutePrefix = string.Empty; // Carga Swagger directamente en la raíz
});

// Desactivado UseHttpsRedirection para evitar conflictos con el proxy de Render
// app.UseHttpsRedirection(); 

app.UseCors("FrontendPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
