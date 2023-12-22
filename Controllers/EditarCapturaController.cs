using Microsoft.AspNetCore.Mvc;
using RinkuCinematografica.Models;
using System.Data.SqlClient;
using System.Data;

namespace RinkuCinematografica.Controllers
{
    public class EditarCapturaController : Controller
    {
        private readonly IConfiguration _configuration;

        public EditarCapturaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> EdicionCaptura()
        {
            // Obtener la lista de roles desde el stored procedure
            List<Rol> roles = await ObtenerListaRoles();
            ViewBag.Roles = roles;

            List<EmpleadoChk> empleados = ObtenerListaEmpleados("");
            ViewBag.Empleados = empleados;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EdicionCaptura(CapturaMensual captura)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("GuardarCapturaMensual", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@CapturaID", captura.CapturaID);
                        command.Parameters.AddWithValue("@EmpleadoID", captura.EmpleadoID);
                        command.Parameters.AddWithValue("@EntregasRealizadas", captura.EntregasRealizadas);
                        command.Parameters.AddWithValue("@BajaLogica", captura.BajaLogica);

                        SqlParameter resultadoParam = new SqlParameter("@Resultado", SqlDbType.NVarChar, 255);
                        resultadoParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(resultadoParam);

                        command.ExecuteNonQuery();

                        string resultado = command.Parameters["@Resultado"].Value.ToString();

                        // Devuelve la respuesta como un objeto JSON
                        return Json(new { success = true, message = resultado });
                    }
                }
            }
            catch (Exception ex)
            {
                // Devuelve la respuesta como un objeto JSON
                return Json(new { success = false, message = $"Error al editar la captura: {ex.Message}" });
            }
        }

        private async Task<List<Rol>> ObtenerListaRoles()
        {
            List<Rol> roles = new List<Rol>();

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("RolesChkBox", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                Rol rol = new Rol
                                {
                                    RolID = reader.GetInt32(0),
                                    NombreRoll = reader.GetString(1)
                                };
                                roles.Add(rol);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la lista de roles: {ex.Message}");
            }

            return roles;
        }

        public IActionResult BuscarCapturas(int bajaLogica)
        {
            List<CapturaMensual> empleados = ObtenerListaCaptura(bajaLogica);
            return Json(empleados);
        }

        [HttpGet]
        public IActionResult AutocompleteEmpleado()
        {
            List<EmpleadoChk> empleados = ObtenerListaEmpleados("");
            return Json(empleados);
        }

        private List<CapturaMensual> ObtenerListaCaptura(int? bajaLogica = 0)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                List<CapturaMensual> CapturasMensuales = new List<CapturaMensual>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("BuscarCapturasMensuales", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@BajaLogica", bajaLogica);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CapturaMensual CapturaMensual = new CapturaMensual
                                {
                                    CapturaID = reader.GetInt32(0),
                                    EmpleadoID = reader.GetInt32(1),
                                    Nombre = reader.GetString(2),
                                    ApellidoP = reader.GetString(3),
                                    AppellidoM = reader.GetString(4),
                                    RollID = reader.GetInt32(5),
                                    NombreRoll = reader.GetString(6),
                                    EntregasRealizadas = reader.GetInt32(7),
                                    Fecha = reader.GetDateTime(8),
                                    BajaLogica = reader.GetInt32(9)
                                };
                                CapturasMensuales.Add(CapturaMensual);
                            }
                        }
                    }
                }

                return CapturasMensuales;
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Error al obtener la lista de empleados: {ex.Message}");
                return new List<CapturaMensual>();
            }
        }

        private List<EmpleadoChk> ObtenerListaEmpleados(string term)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                List<EmpleadoChk> empleados = new List<EmpleadoChk>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("EmpleadoChkBox", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmpleadoChk empleado = new EmpleadoChk
                                {
                                    EmpleadoID = reader.GetInt32(0),
                                    Nombre = reader.GetString(1)
                                };
                                empleados.Add(empleado);
                            }
                        }
                    }
                }

                return empleados;
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Error al obtener la lista de empleados: {ex.Message}");
                return new List<EmpleadoChk>();
            }
        }
    }
}
