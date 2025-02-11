using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SqlInjectionVulnerable.Data;
 

var builder = WebApplication.CreateBuilder(args);

// 🔹 Read connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";  // Redirect to login if unauthorized
    });


builder.Services.AddControllersWithViews();

var app = builder.Build();


app.UseRouting();
app.UseAuthentication(); // Enable authentication (cookie-based)
app.UseAuthorization();
app.UseStaticFiles();  // Enable serving CSS, JS, images from wwwroot


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
