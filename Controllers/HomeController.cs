using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SqlInjectionVulnerable.Models;
using System.Diagnostics;

namespace SqlInjectionVulnerable.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger; _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        private string _connectionString;

        
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]  // 👈 Only logged-in users can access
        public IActionResult Dashboard()
        {
            return View();
        }
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword)
        {
            string username = User.Identity.Name;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // ⚠ SQL Injection Vulnerability: Concatenating user input into the query
                string sqlQuery = $"UPDATE Users SET Password = '{newPassword}' WHERE Username = '{username}' AND Password = '{oldPassword}'";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ViewBag.Message = "Password changed successfully!";
                    }
                    else
                    {
                        ViewBag.Message = "Old password is incorrect.";
                    }
                }
            }

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}