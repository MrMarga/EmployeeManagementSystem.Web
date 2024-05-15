namespace backend_app.Model
{
    public class LogInRequest
    {
        public required string Email { get; set; }

        public required string Password { get; set; }
    
        public required string Role { get; set; }
    }

    public class LogInResponse
    {

        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
