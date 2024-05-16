using Microsoft.AspNetCore.Identity;

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

        public UserLoginInfromation Data { get; set; }
    }

    public class UserLoginInfromation
    {
        public string UserID;

        public string UserName;

        public string Role;
    }
}
