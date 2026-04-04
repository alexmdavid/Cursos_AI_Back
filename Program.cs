using Cursos_AI_Back.Data;
using Cursos_AI_Back.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // O prueba con .WithOrigins("https://alexmdavid.github.io")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// --- MIDDLEWARE (EL ORDEN ES VITAL) ---

// 1. Swagger (si estás en dev)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. CORS DEBE IR AQUÍ (Antes de Routing y MapControllers)
app.UseCors("AllowAll");

// 3. Comenta esto temporalmente para descartar que la redirección rompa el CORS
// app.UseHttpsRedirection(); 

app.UseAuthorization();

// 4. Mapeo al final
app.MapControllers();

app.Run();