namespace backend_app.Model
{
    // Models/Role.cs
    public class Role
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }

    // Models/UserRole.cs
    public class UserRole
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
