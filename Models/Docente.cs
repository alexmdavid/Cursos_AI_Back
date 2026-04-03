namespace Cursos_AI_Back.Models
{
    public class Docente
    {
        public int Id { get; set; }

        public string Nombres { get; set; }
        public string Apellidos { get; set; }

        public string Correo { get; set; }
        public string Telefono { get; set; }

        public string Ciudad { get; set; }
        public string Pais { get; set; }

        public string Institucion { get; set; }
        public string Cargo { get; set; }

        public string AreaEnsenanza { get; set; }
        public string NivelEducativo { get; set; }

        public bool AceptaComunicaciones { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
