using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SqlInjectionVulnerable.Data;
using SqlInjectionVulnerable.Models;
using System.Security.Claims;

namespace SqlInjectionVulnerable.Controllers
{
    public class ItemsController : Controller
    {
        private readonly string _connectionString;

        public ItemsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: Items
        public IActionResult Index()
        {
            List<Item> items = new List<Item>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Fetch all items from the database
                string query = "SELECT Id, Name, Price FROM Items";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new Item
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2)
                            });
                        }
                    }
                }
            }

            return View(items);
        }

        // POST: Items/Search
        [HttpPost]
        public IActionResult Search(string searchTerm)
        {
            List<Item> items = new List<Item>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // ⚠ Vulnerable to SQL Injection - UNION attack
                string sqlQuery = $"SELECT Id, Name, Description, Price FROM Items WHERE Name LIKE '%{searchTerm}%'";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new Item
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                Price = reader.GetDecimal(3)
                            });
                        }
                    }
                }
            }

            return View("Index", items); // Return to the Index view with the filtered items
        }
    }
}
