using Microsoft.AspNetCore.Identity;

namespace backend_app.Model
{
    public class SignUpRequest
    {
        public required string Username { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public required string ConfirmPassword { get; set; }

        public required string Role { get; set; }
        
    }

    public class SignUpResponse
    {
       
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;

        
    }


}
