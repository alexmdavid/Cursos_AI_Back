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
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseCors("AllowAll");


// 3. OJO AQUÍ: En Render/Docker a veces HttpsRedirection da problemas si no tienes SSL configurado internamente. 
// Render ya te da HTTPS desde afuera, así que puedes comentarlo o dejarlo así:
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.Run();