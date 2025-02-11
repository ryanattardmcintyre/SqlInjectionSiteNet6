using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SqlInjectionVulnerable.Data;
using System.Security.Claims;

namespace SqlInjectionVulnerable.Controllers
{
    public class LoginController : Controller
    {
        private   string _connectionString;

        // Inject IConfiguration to read from appsettings.json
        public LoginController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // ⚠ SQL Injection Vulnerable Query
                string sqlQuery = $"SELECT Id FROM Users WHERE Username = '{username}' AND Password = '{password}'";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    var userId = cmd.ExecuteScalar();

                    if (userId != null)
                    {
                        // ✅ Authentication successful
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, username),
                            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                        };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        return RedirectToAction("Dashboard", "Home");  // Redirect to a protected page
                    }
                    else
                    {
                        ViewBag.Message = "Invalid credentials!";
                    }
                }
            }

            return View("Index");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }
    }
}
