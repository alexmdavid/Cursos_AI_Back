using Cursos_AI_Back.Models;
using Microsoft.EntityFrameworkCore;

namespace Cursos_AI_Back.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Docente> Docentes { get; set; }
    }
}
