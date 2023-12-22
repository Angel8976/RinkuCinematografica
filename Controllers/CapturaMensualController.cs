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
    public class CapturaMensualController : Controller
    {
        private readonly IConfiguration _configuration;

        public CapturaMensualController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GuardarCapturaMensual()
        {
            List<EmpleadoChk> empleados = ObtenerListaEmpleados("");
            ViewBag.Empleados = empleados;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> GuardarCapturaMensual(CapturaMensual capturaMensual)
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

                        command.Parameters.AddWithValue("@CapturaID", 0);
                        command.Parameters.AddWithValue("@EmpleadoID", capturaMensual.EmpleadoID);
                        command.Parameters.AddWithValue("@EntregasRealizadas", capturaMensual.EntregasRealizadas);
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
                return Json(new { success = false, message = $"Error al guardar la captura mensual: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult AutocompleteEmpleado()
        {
            List<EmpleadoChk> empleados = ObtenerListaEmpleados("");
            return Json(empleados);
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
