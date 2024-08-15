using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Employee_Manage_System.Models;
using Employee_Manage_System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Employee_Manage_System.Pages
{
    public class EditEmployeeModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditEmployeeModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; }

        public SelectList GenderOptions { get; set; }
        public SelectList EmploymentTypeOptions { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Load employee data
            Employee = await _context.Employees.FindAsync(id);
            if (Employee == null)
            {
                return NotFound();
            }

            // Initialize dropdown options
            GenderOptions = new SelectList(new List<string> { "Male", "Female", "Other" }, Employee.Gender);
            EmploymentTypeOptions = new SelectList(new List<string> { "FullTime", "PartTime", "Contract" }, Employee.EmploymentType);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(Employee.EmployeeId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Home");
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
