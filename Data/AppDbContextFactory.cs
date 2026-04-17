using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Cursos_AI_Back.Data;

namespace Cursos_AI_Back.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseNpgsql(
                "Host=194.146.13.211;Port=5432;Database=cursos-ia-db;Username=postgres;Password=0000"
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}