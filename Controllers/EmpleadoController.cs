using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RinkuCinematografica.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace RinkuCinematografica.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly IConfiguration _configuration;

        public EmpleadoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> GuardarEmpleado()
        {
            // Obtener la lista de roles desde el stored procedure
            List<Rol> roles = await ObtenerListaRoles();
            ViewBag.Roles = roles;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GuardarEmpleado(Empleado empleado)
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

                        command.Parameters.AddWithValue("@EmpleadoID", 0);
                        command.Parameters.AddWithValue("@Nombre", empleado.Nombre);
                        command.Parameters.AddWithValue("@ApellidoP", empleado.ApellidoP);
                        command.Parameters.AddWithValue("@ApellidoM", empleado.AppellidoM);
                        command.Parameters.AddWithValue("@RollID", empleado.RollID);
                        command.Parameters.AddWithValue("@BajaLogica", 0);

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
                return Json(new { success = false, message = $"Error al guardar el empleado: {ex.Message}" });
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
    }
}