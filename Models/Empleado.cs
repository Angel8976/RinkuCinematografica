namespace RinkuCinematografica.Models
{
    public class Empleado
    {
        public int EmpleadoID { get; set; }
        public string? Nombre { get; set; }
        public string? ApellidoP { get; set; }
        public string? AppellidoM { get; set; }
        public int RollID { get; set; }
        public string NombreCompleto => $"{Nombre} {ApellidoP} {AppellidoM}";
        public string? NombreRoll { get; set; }
        public int BajaLogica { get; set; }
    }
}
