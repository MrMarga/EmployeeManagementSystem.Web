namespace backend_app.DTO
{
    public class SignUpRequest
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }
        public IFormFile ImageFile { get; set; } 
    }

    public class SignUpResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        
    }
}
