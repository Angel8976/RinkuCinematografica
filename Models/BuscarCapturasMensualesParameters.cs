namespace RinkuCinematografica.Models
{
    public class BuscarCapturasMensualesParameters
    {
        public int? CapturaID { get; set; }
        public int? EmpleadoID { get; set; }
        public string? Nombre { get; set; }
        public string? ApellidoP { get; set; }
        public string? AppellidoM { get; set; }
        public int? Anio { get; set; }
        public int? Mes { get; set; }
        public int? BajaLogica { get; set; }
    }
}
