using Microsoft.AspNetCore.Mvc;
using RinkuCinematografica.Models;
using System.Data.SqlClient;
using System.Data;

namespace RinkuCinematografica.Controllers
{
    public class ReporteMensualController : Controller
    {
        private readonly IConfiguration _configuration;

        public ReporteMensualController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> GenerarReporteMensual()
        {
            // Obtener la lista de roles desde el stored procedure
            //List<Rol> roles = await ObtenerListaRoles();
            //ViewBag.Roles = roles;

            List<ReporteSueldosResult> Reportes = ObtenerReporteMensual();

            return View();
        }

        public IActionResult GenerarReporte(int? Mes = 0, int? Anio = 0)
        {
            List<ReporteSueldosResult> reportes = ObtenerReporteMensual(Mes, Anio);
            return Json(reportes);
        }

        private List<ReporteSueldosResult> ObtenerReporteMensual(int? Mes = 0, int? Anio = 0)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                List<ReporteSueldosResult> Reportes = new List<ReporteSueldosResult>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("GenerarReporteSueldos", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Mes", Mes);
                        command.Parameters.AddWithValue("@Anio", Anio);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReporteSueldosResult Reporte = new ReporteSueldosResult
                                {
                                    EmpleadoID = reader.GetInt32(0),
                                    Nombre = reader.GetString(1),
                                    ApellidoP = reader.GetString(2),
                                    ApellidoM = reader.GetString(3),
                                    RollID = reader.GetInt32(4),
                                    NombreRoll = reader.GetString(5),
                                    EntregasRealizadas = reader.GetInt32(6),
                                    Fecha = reader.GetDateTime(7),
                                    SueldoBase = reader.GetDecimal(8),
                                    BonoPorHora = reader.GetDecimal(9),
                                    RetencionesISR = reader.GetDecimal(10),
                                    ValesDeDespensa = reader.GetDecimal(11),
                                    SueldoTotal = reader.GetDecimal(12),
                                };
                                Reportes.Add(Reporte);
                            }
                        }
                    }
                }

                return Reportes;
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Error al obtener reporte: {ex.Message}");
                return new List<ReporteSueldosResult>();
            }
        }
    }
}
