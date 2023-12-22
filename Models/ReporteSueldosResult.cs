namespace RinkuCinematografica.Models
{
    public class ReporteSueldosResult
    {
        public int EmpleadoID { get; set; }
        public string? Nombre { get; set; }
        public string? ApellidoP { get; set; }
        public string? ApellidoM { get; set; }
        public int RollID { get; set; }
        public string? NombreRoll { get; set; }
        public int EntregasRealizadas { get; set; }
        public DateTime Fecha { get; set; }
        public decimal SueldoBase { get; set; }
        public decimal BonoPorHora { get; set; }
        public decimal RetencionesISR { get; set; }
        public decimal ValesDeDespensa { get; set; }
        public decimal SueldoTotal { get; set; }
    }
}
