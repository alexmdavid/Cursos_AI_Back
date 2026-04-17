using Cursos_AI_Back.Data;
using Cursos_AI_Back.Models;
using Cursos_AI_Back.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            try
            {
                docente.FechaRegistro = DateTime.UtcNow;
                _context.Docentes.Add(docente);
                await _context.SaveChangesAsync();

                // En el método Registrar del controlador:
                Console.WriteLine("--- Intentando enviar correo a: " + docente.Correo);
                try
                {
                    await _emailService.EnviarCorreoRegistroAsync(docente.Correo, docente.Nombres);
                    Console.WriteLine("--- Correo enviado sin excepciones.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("--- ERROR AL ENVIAR CORREO: " + ex.Message);
                }

                return Ok(new { mensaje = "Docente registrado y correo enviado" });
            }
            catch (Exception exGlobal)
            {
                return StatusCode(500, new { error = exGlobal.Message, detalle = exGlobal.InnerException?.Message });
            }
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


        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var docentes = await _context.Docentes.ToListAsync();
            return Ok(docentes);
        }
    }


}
