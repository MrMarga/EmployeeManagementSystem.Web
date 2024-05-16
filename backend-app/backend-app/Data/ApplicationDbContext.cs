using backend_app.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace backend_app.Data
{
    public class ApplicationDbContext : IdentityDbContext
    { 
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<UserDetails> UserDetails { get; set; }

        public DbSet<Employee> Employees { get; set; }


    }
       
   }  


