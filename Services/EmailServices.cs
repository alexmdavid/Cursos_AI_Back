using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace Cursos_AI_Back.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarCorreoAsync(string para, string asunto, string contenidoHtml)
        {
            var email = new MimeMessage();
            // Remitente: Debe ser el mismo correo que autentica
            email.From.Add(new MailboxAddress("Playdiom Community", "ruidiazcarrascala@gmail.com"));
            email.To.Add(MailboxAddress.Parse(para));
            email.Subject = asunto;

            email.Body = new TextPart("html") { Text = contenidoHtml };

            using var smtp = new SmtpClient();

            try
            {
                // --- CONFIGURACIÓN PARA GMAIL ---
                var host = "smtp.gmail.com";
                var port = 587;
                var user = "ruidiazcarrascala@gmail.com";
                var pass = "qwhrmwetmmavvwpm"; // Tu App Password de 16 caracteres

                // 1. BYPASS DE SEGURIDAD (Soluciona el error de revocación en redes locales/unimag)
                smtp.CheckCertificateRevocation = false;
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                smtp.Timeout = 30000; // Aumentamos a 30s por si la red es lenta

                // 2. CONEXIÓN Y ENVÍO
                Console.WriteLine($"--- Intentando enviar correo a: {para}...");

                await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(user, pass);
                await smtp.SendAsync(email);

                Console.WriteLine($"--- ¡ÉXITO! Correo enviado vía Gmail a: {para}");
            }
            catch (Exception ex)
            {
                // Imprimimos el error detallado en consola
                Console.WriteLine($"--- ERROR CRÍTICO GMAIL: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"--- DETALLE ADICIONAL: {ex.InnerException.Message}");
                }
                throw; // Lanzamos la excepción para que el controlador sepa que falló
            }
            finally
            {
                if (smtp.IsConnected) await smtp.DisconnectAsync(true);
            }
        }

        public async Task EnviarCorreoRegistroAsync(string para, string nombre)
        {
            string cuerpoHtml;
            try
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Templates", "correo.html");
                if (File.Exists(path))
                {
                    cuerpoHtml = await File.ReadAllTextAsync(path);
                    cuerpoHtml = cuerpoHtml.Replace("{{nombre}}", nombre);
                }
                else
                {
                    cuerpoHtml = $"<div style='font-family: sans-serif;'><h1>¡Hola {nombre}!</h1><p>Gracias por registrarte en Cursos IA.</p></div>";
                }
            }
            catch
            {
                cuerpoHtml = $"<h1>Hola {nombre}</h1><p>Registro exitoso.</p>";
            }

            // Llamada al método de envío
            await EnviarCorreoAsync(para, "🎓 Acceso a Cursos IA 2026", cuerpoHtml);
        }
    }
}