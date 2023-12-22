using Microsoft.AspNetCore.Mvc;
using RinkuCinematografica.Models;
using System.Data.SqlClient;
using System.Data;

namespace RinkuCinematografica.Controllers
{
    public class EditarEmpleadoController : Controller
    {
        private readonly IConfiguration _configuration;

        public EditarEmpleadoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> EdicionEmpleado()
        {
            // Obtener la lista de roles desde el stored procedure
            List<Rol> roles = await ObtenerListaRoles();
            ViewBag.Roles = roles;

            List<Empleado> empleados = ObtenerListaEmpleados();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EdicionEmpleado(Empleado empleado)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("GuardarEmpleado", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@EmpleadoID", empleado.EmpleadoID);
                        command.Parameters.AddWithValue("@Nombre", empleado.Nombre);
                        command.Parameters.AddWithValue("@ApellidoP", empleado.ApellidoP);
                        command.Parameters.AddWithValue("@ApellidoM", empleado.AppellidoM);
                        command.Parameters.AddWithValue("@RollID", empleado.RollID);
                        command.Parameters.AddWithValue("@BajaLogica", empleado.BajaLogica);

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
                return Json(new { success = false, message = $"Error al editar el empleado: {ex.Message}" });
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

        public IActionResult BuscarEmpleados(int bajaLogica)
        {
            List<Empleado> empleados = ObtenerListaEmpleados(bajaLogica);
            return Json(empleados);
        }

        private List<Empleado> ObtenerListaEmpleados(int? bajaLogica = 0)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                List<Empleado> empleados = new List<Empleado>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("BuscarEmpleados", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@BajaLogica", bajaLogica);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Empleado empleado = new Empleado
                                {
                                    EmpleadoID = reader.GetInt32(0),
                                    Nombre = reader.GetString(1),
                                    ApellidoP = reader.GetString(2),
                                    AppellidoM = reader.GetString(3),
                                    RollID = reader.GetInt32(4),
                                    NombreRoll = reader.GetString(5),
                                    BajaLogica = reader.GetInt32(6),
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
                return new List<Empleado>();
            }
        }
    }
}
