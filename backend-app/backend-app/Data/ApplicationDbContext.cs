using backend_app.Model;
using Microsoft.EntityFrameworkCore;

namespace backend_app.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<UserDetails> UserDetails { get; set; }

        public DbSet<Employee> Employees { get; set; }


    }
       
   }  


