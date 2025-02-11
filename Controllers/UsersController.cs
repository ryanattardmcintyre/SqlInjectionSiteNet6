using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace SqlInjectionVulnerable.Controllers
{
    public class UsersController : Controller
    {
        private readonly string _connectionString;

        public UsersController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public IActionResult Register(string username, string password, int mobile)
        {
            // ⚠ SQL Injection Vulnerable Query
            string sqlQuery = $"INSERT INTO Users (Username, Password, Mobile) VALUES ('{username}', '{password}', '{mobile}')";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ViewBag.Message = "User registered successfully.";
                        }
                        else
                        {
                            ViewBag.Message = "Registration failed!";
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = $"Error: {ex.Message}";
                    }
                }
            }

            return View();
        }
    }

}
