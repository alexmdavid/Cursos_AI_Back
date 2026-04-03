
namespace Cursos_AI_Back.Services
{
    public interface IEmailService
    {
        Task EnviarCorreoAsync(string para, string asunto, string contenidoHtml);
        Task EnviarCorreoRegistroAsync(string para, string nombre);
    }
}
