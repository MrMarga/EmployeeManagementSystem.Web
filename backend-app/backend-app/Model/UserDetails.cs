using System.ComponentModel.DataAnnotations;

namespace backend_app.Model
{
    public class UserDetails
    {
        [Key]
        public int UserId { get; set; }

        public required string Username { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; } 
        public required string Password { get; set; }
        public required string Role { get; set; }

    }
}
