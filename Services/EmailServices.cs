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
            // Usamos el usuario de la configuración para el remitente
            var user = _config["SmtpSettings:User"];
            var pass = _config["SmtpSettings:Pass"];
            var host = _config["SmtpSettings:Host"];
            var port = int.Parse(_config["SmtpSettings:Port"] ?? "587");

            email.From.Add(new MailboxAddress("Playdiom Community", user));
            email.To.Add(MailboxAddress.Parse(para));
            email.Subject = asunto;

            email.Body = new TextPart("html") { Text = contenidoHtml };

            using var smtp = new SmtpClient();
            try
            {
                smtp.CheckCertificateRevocation = false;
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

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