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

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var docente = await _context.Docentes.FindAsync(id);

            if (docente == null)
                return NotFound(new { mensaje = "Docente no encontrado" });

            return Ok(docente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Docente docente)
        {
            var existente = await _context.Docentes.FindAsync(id);

            if (existente == null)
                return NotFound(new { mensaje = "No existe" });

            existente.Nombres = docente.Nombres;
            existente.Apellidos = docente.Apellidos;
            existente.Correo = docente.Correo;
            existente.Telefono = docente.Telefono;
            existente.Ciudad = docente.Ciudad;
            existente.Pais = docente.Pais;
            existente.Institucion = "";
            existente.Cargo = "";
            existente.AreaEnsenanza = "";
            existente.NivelEducativo = "";
            existente.AceptaComunicaciones = docente.AceptaComunicaciones;

            await _context.SaveChangesAsync();

            return Ok(existente);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var docente = await _context.Docentes.FindAsync(id);

            if (docente == null)
                return NotFound(new { mensaje = "No existe" });

            _context.Docentes.Remove(docente);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Eliminado correctamente" });
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarPorCorreo(string correo)
        {
            var resultados = await _context.Docentes
                .Where(d => d.Correo.Contains(correo))
                .ToListAsync();

            return Ok(resultados);
        }

        [HttpGet("total")]
        public async Task<IActionResult> Total()
        {
            var total = await _context.Docentes.CountAsync();
            return Ok(new { total });
        }



    }
}
