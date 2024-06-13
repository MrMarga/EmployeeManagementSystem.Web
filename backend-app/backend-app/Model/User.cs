using System.ComponentModel.DataAnnotations.Schema;

namespace backend_app.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string ProfileImagePath { get; set; }

        [NotMapped] 
        public IFormFile? ImageFile { get; set; } 
        public ICollection<UserRole> UserRoles { get; set; }

    }

}
