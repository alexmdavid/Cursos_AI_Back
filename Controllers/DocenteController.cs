using Cursos_AI_Back.Data;
using Cursos_AI_Back.Models;
using Cursos_AI_Back.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Cursos_AI_Back.Controllers
{

    [ApiController]
    [Route("api/docentes")]
    public class DocentesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public DocentesController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] Docente docente)
        {
            docente.FechaRegistro = DateTime.UtcNow;

            _context.Docentes.Add(docente);
            await _context.SaveChangesAsync();

            // 📩 CORREO DE PRUEBA 
            await _emailService.EnviarCorreoRegistroAsync(
            docente.Correo,
            docente.Nombres
        );

            return Ok(new { mensaje = "Docente registrado y correo enviado" });
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Conexion funcionando");
        }

        [HttpPost("prueba")]
        public async Task<IActionResult> EnviarPrueba()
        {
            try
            {
                await _emailService.EnviarCorreoAsync(
                    "ruidiazcarrascala@gmail.com",
                    "Prueba directa",
                    "<h1>Hola, este es un correo de prueba 🚀</h1>"
                );

                return Ok(new { mensaje = "Correo enviado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


    }


}
