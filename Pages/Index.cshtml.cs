using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Employee_Manage_System.Data;
using Employee_Manage_System.Models;

namespace Employee_Manage_System.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }


        public string CurrentPath { get; private set; } 

        public async Task<IActionResult> OnGetAsync()
        {
            CurrentPath = HttpContext.Request.Path;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _context.Users
                .Where(u => u.Username == Username && u.Password == Password)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                ErrorMessage = "Invalid login attempt.";
                return Page();
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true 
                });

            return RedirectToPage("/Home", new { successMessage = "Login successful!" });
        }


    }
}
