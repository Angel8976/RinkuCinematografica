namespace RinkuCinematografica.Models
{
    public class CapturaMensual
    {
        public int CapturaID { get; set; }
        public int EmpleadoID { get; set; }
        public string? Nombre { get; set; }
        public string? ApellidoP { get; set; }
        public string? AppellidoM { get; set; }
        public int RollID { get; set; }
        public string? NombreRoll { get; set; }
        public int? EntregasRealizadas { get; set; }
        public DateTime Fecha { get; set; }
        public int? BajaLogica { get; set; }
    }
}
