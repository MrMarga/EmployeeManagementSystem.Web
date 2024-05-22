using System.ComponentModel.DataAnnotations;

namespace backend_app.Model
{
    public class User
    {
        public int Id { get; set; }

        public required string Name { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public  ICollection<UserRole> UserRoles { get; set; }

    }
}
