using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Employee_Manage_System.Data;
using Employee_Manage_System.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.SkiaSharp;
using System.IO;
using OfficeOpenXml;

namespace Employee_Manage_System.Pages
{
    public class HomeModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public HomeModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Employee> Employees { get; set; }

        [BindProperty(SupportsGet = true)]
        public string[] Gender { get; set; }
        [BindProperty(SupportsGet = true)]
        public string[] EmploymentType { get; set; }
        [BindProperty(SupportsGet = true)]
        public string[] Position { get; set; }
        [BindProperty(SupportsGet = true)]
        public string[] Department { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Query { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<Employee> query = _context.Employees;
            var lowerQuery = Query?.ToLower() ?? string.Empty;

            if (!string.IsNullOrEmpty(Query))
            {
                query = query.Where(e => e.FirstName.Contains(Query) ||
                                          e.LastName.Contains(Query) ||
                                          e.Email.Contains(Query) ||
                                          e.EmployeeId.ToString().Contains(lowerQuery) ||
                                          e.PhoneNo.Contains(Query));
            }

            if (Gender != null && Gender.Length > 0)
            {
                query = query.Where(e => Gender.Contains(e.Gender));
            }
            if (EmploymentType != null && EmploymentType.Length > 0)
            {
                query = query.Where(e => EmploymentType.Contains(e.EmploymentType));
            }
            if (Position != null && Position.Length > 0)
            {
                query = query.Where(e => Position.Contains(e.Position));
            }
            if (Department != null && Department.Length > 0)
            {
                query = query.Where(e => Department.Contains(e.Department));
            }

            Employees = await query.ToListAsync();
        }

        public async Task<IActionResult> OnGetDeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        [HttpPost]
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Employee deleted successfully.";
            return RedirectToPage();
        }


        public IActionResult OnPostExportToExcel()
        {
            var employees = _context.Employees.ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employees");
                AddEmployeeData(worksheet, employees);

                using (var stream = new MemoryStream())
                {
                    package.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
                }
            }
        }

        public IActionResult OnPostGenerateGraph()
        {
            var employees = _context.Employees.ToList();
            var plotModel = CreatePlotModel(employees);

            using (var stream = new MemoryStream())
            {
                var pngExporter = new PngExporter { Width = 800, Height = 600 };
                pngExporter.Export(plotModel, stream);

                var content = stream.ToArray();
                return File(content, "image/png", "Employee_Chart.png");
            }
        }

        private void AddEmployeeData(ExcelWorksheet worksheet, IList<Employee> employees)
        {
            var headers = new[] { "Employee ID", "First Name", "Last Name", "Gender", "Email", "Phone No.", "Employment Type", "Position", "Department", "Date Joined" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            for (int i = 0; i < employees.Count; i++)
            {
                var employee = employees[i];
                worksheet.Cells[i + 2, 1].Value = employee.EmployeeId;
                worksheet.Cells[i + 2, 2].Value = employee.FirstName;
                worksheet.Cells[i + 2, 3].Value = employee.LastName;
                worksheet.Cells[i + 2, 4].Value = employee.Gender;
                worksheet.Cells[i + 2, 5].Value = employee.Email;
                worksheet.Cells[i + 2, 6].Value = employee.PhoneNo;
                worksheet.Cells[i + 2, 7].Value = employee.EmploymentType;
                worksheet.Cells[i + 2, 8].Value = employee.Position;
                worksheet.Cells[i + 2, 9].Value = employee.Department;
                worksheet.Cells[i + 2, 10].Value = employee.DateJoined.ToString("yyyy-MM-dd");
            }
        }

        private PlotModel CreatePlotModel(IList<Employee> employees)
        {
            var plotModel = new PlotModel { Title = "Employees Over Time", Background = OxyColors.White };

            // Prepare data: count employees per date
            var dateEmployeeCounts = employees
                .GroupBy(e => e.DateJoined.Date) // Group by Date only
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date) // Ensure dates are in order
                .ToList();

            var lineSeries = new LineSeries
            {
                Title = "Number of Employees",
                StrokeThickness = 2,
                Color = OxyColors.SkyBlue,
                MarkerType = MarkerType.Circle, // Fully qualified name to resolve ambiguity
                MarkerSize = 4
            };

            foreach (var item in dateEmployeeCounts)
            {
                lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(item.Date), item.Count));
            }

            plotModel.Series.Add(lineSeries);

            // Configure x-axis as DateTimeAxis
            plotModel.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "MM/dd/yyyy",
                Title = "Date Joined",
                Minimum = DateTimeAxis.ToDouble(dateEmployeeCounts.FirstOrDefault()?.Date ?? DateTime.Now.AddMonths(-1)),
                Maximum = DateTimeAxis.ToDouble(dateEmployeeCounts.LastOrDefault()?.Date ?? DateTime.Now)
            });

            // Configure y-axis for number of employees
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Title = "Number of Employees"
            });

            return plotModel;
        }
    }
}