using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlInjectionVulnerable.Data;
using SqlInjectionVulnerable.Models;
using System.Security.Claims;

namespace SqlInjectionVulnerable.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Comment/Index
        [AllowAnonymous]
        public IActionResult Index()
        {
            var comments = _context.Comments.OrderByDescending(c => c.CreatedAt).ToList();
            return View(comments);
        }

        // GET: Comment/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Comment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string content)
        {
            if (ModelState.IsValid)
            {
                var comment = new Comment
                {
                    Content = content,  // No sanitization is done here, so XSS is allowed
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    CreatedAt = DateTime.Now
                };

                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }

}
