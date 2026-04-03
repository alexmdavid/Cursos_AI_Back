namespace Cursos_AI_Back.Services
{
    using MailKit.Net.Smtp;
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

            await smtp.ConnectAsync("smtp.gmail.com", 587, false);

            await smtp.AuthenticateAsync("joelarga2020@gmail.com", "mqkildfdumgjuohv");

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }

        public async Task EnviarCorreoRegistroAsync(string para, string nombre)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "correo.html");

            var html = await File.ReadAllTextAsync(path);

            // Personalización
            html = html.Replace("{{nombre}}", nombre);

            await EnviarCorreoAsync(
                para,
                "🎓 Acceso a Cursos IA 2026",
                html
            );
        }
    }
}
