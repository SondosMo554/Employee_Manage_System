using System;
using System.Linq;
using System.Threading.Tasks;
using Employee_Manage_System.Models;

namespace Employee_Manage_System.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            // Clear existing data
            context.Employees.RemoveRange(context.Employees);

            // Seed data
            var employees = GenerateEmployees(50);

            context.Employees.AddRange(employees);
            await context.SaveChangesAsync();
        }


        private static Employee[] GenerateEmployees(int count)
        {
            var random = new Random();
            var departments = new[] { "IT", "Marketing", "HR", "Finance", "Sales", "Consulting" };
            var positions = new[] { "Developer", "Designer", "Project Manager", "Analyst", "Sales Representative", "Consultant", "HR Manager", "Intern" };
            var employmentTypes = new[] { "Full-time", "Part-time", "Contract", "Internship" };
            var genders = new[] { "Male", "Female" };

            var employees = Enumerable.Range(1, count).Select(i =>
            {
                return new Employee
                {
                    FirstName = $"FirstName{i}",
                    LastName = $"LastName{i}",
                    Gender = genders[random.Next(genders.Length)],
                    Email = $"email{i}@example.com",
                    PhoneNo = $"+60 555-{random.Next(1000, 9999)}", // Prepend +60 for Malaysia
                    EmploymentType = employmentTypes[random.Next(employmentTypes.Length)],
                    Position = positions[random.Next(positions.Length)],
                    Department = departments[random.Next(departments.Length)],
                    DateJoined = DateTime.Now.AddDays(-random.Next(1, 365))
                };
            }).ToArray();

            return employees;
        }


    }
}
