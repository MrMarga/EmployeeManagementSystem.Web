using System.ComponentModel.DataAnnotations;

namespace backend_app.Model
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        public required string FullName { get; set; }

        public required string Email { get; set; }

        public long Phone { get; set; }

      
    }
}
