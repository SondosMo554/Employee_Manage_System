using Microsoft.EntityFrameworkCore;
using Employee_Manage_System.Models;
using System.Collections.Generic;


namespace Employee_Manage_System.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }

    }


}
