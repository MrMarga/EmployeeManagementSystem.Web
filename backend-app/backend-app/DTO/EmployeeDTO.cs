namespace backend_app.DTO
{
    public class EmployeeDTO
    {
        public required string FullName { get; set; }

        public required string Email { get; set; }

        public long Phone { get; set; }
    }
}
