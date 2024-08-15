using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Employee_Manage_System.Models;
using Employee_Manage_System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using PhoneNumbers;


namespace Employee_Manage_System.Pages
{
    public class CreateEmployeeModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateEmployeeModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; }

        public SelectList GenderOptions { get; set; }
        public SelectList EmploymentTypeOptions { get; set; }
        public SelectList PositionOptions { get; set; }
        public SelectList DepartmentOptions { get; set; }

        public void OnGet()
        {
            GenderOptions = new SelectList(new List<string> { "Male", "Female" });
            EmploymentTypeOptions = new SelectList(new List<string> { "Full-time", "Part-time", "Contract", "Internship" });
            PositionOptions = new SelectList(new List<string> { "Developer", "Designer", "Project Manager", "Analyst", "Sales Representative", "Consultant", "HR Manager", "Intern" });
            DepartmentOptions = new SelectList(new List<string> { "IT", "Marketing", "HR", "Finance", "Sales", "Consulting" });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Employees.Add(Employee);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Home"); 
        }
    }
}
