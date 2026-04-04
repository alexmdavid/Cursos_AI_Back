namespace Cursos_AI_Back.Services
{
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;

    public class EmailService : IEmailService
    {
        public async Task EnviarCorreoAsync(string para, string asunto, string contenidoHtml)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress("Test", "joelarga2020@gmail.com"));
            email.To.Add(MailboxAddress.Parse(para));
            email.Subject = asunto;

            email.Body = new TextPart("html")
            {
                Text = contenidoHtml
            };

            using var smtp = new SmtpClient();

            
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
            smtp.Timeout = 30000; // 30 segundos de espera
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await smtp.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);

            //await smtp.AuthenticateAsync("joelarga2020@gmail.com", "mqkildfdumgjuohv");
            var pass = Environment.GetEnvironmentVariable("GMAIL_PASSWORD");
            await smtp.AuthenticateAsync("joelarga2020@gmail.com", pass);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }

        private readonly IWebHostEnvironment _env;

        public EmailService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task EnviarCorreoRegistroAsync(string para, string nombre)
        {
            string cuerpoHtml;
            try
            {
                // Intenta leer el archivo
                var path = Path.Combine(AppContext.BaseDirectory, "Templates", "correo.html");
                if (File.Exists(path))
                {
                    cuerpoHtml = await File.ReadAllTextAsync(path);
                    cuerpoHtml = cuerpoHtml.Replace("{{nombre}}", nombre);
                }
                else
                {
                    // SI NO EXISTE, NO LANZAMOS ERROR, usamos un texto base
                    cuerpoHtml = $"<h1>Hola {nombre}</h1><p>Gracias por registrarte.</p>";
                    Console.WriteLine("Aviso: No se encontró la plantilla, usando HTML de respaldo.");
                }
            }
            catch (Exception ex)
            {
                cuerpoHtml = $"<h1>Hola {nombre}</h1><p>Registro exitoso.</p>";
                Console.WriteLine($"Error con la plantilla: {ex.Message}");
            }

            await EnviarCorreoAsync(para, "🎓 Acceso a Cursos IA 2026", cuerpoHtml);
        }
    }
}
