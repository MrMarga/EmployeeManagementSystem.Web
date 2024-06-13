namespace backend_app.DTO
{
    public class UserDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string ProfileImagePath { get; set; }
        public ICollection<UserRoleDTO> Roles { get; set; } 
    }

    public class UserRoleDTO
    {
        public RoleDTO Role { get; set; }
    }

    public class RoleDTO
    {
        public string Type { get; set; }

    }
}
