using Microsoft.AspNetCore.Identity;

namespace backend_app.Model
{
    public class LogInRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class LogInResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public AuthTokens Tokens { get; set; }
    }
}
