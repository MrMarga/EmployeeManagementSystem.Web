using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using backend_app.Model;
using backend_app.UserRepository;

namespace backend_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserServices _userService;

        public AuthController(IUserServices userService)
        {
            _userService = userService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUpRequest signUpRequest)
        {
            if (signUpRequest.Password != signUpRequest.ConfirmPassword)
            {
                return BadRequest(new SignUpResponse { IsSuccess = false, Message = "Passwords do not match" });
            }

            var isSuccess = await _userService.CreateUserAsync(signUpRequest.Name, signUpRequest.Username, signUpRequest.Email, signUpRequest.Password, signUpRequest.Role);

            if (isSuccess)
            {
                return Ok(new SignUpResponse { IsSuccess = true, Message = "User created successfully" });
            }

            return BadRequest(new SignUpResponse { IsSuccess = false, Message = "User already exists" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn(LogInRequest logInRequest)
        {
            var user = await _userService.AuthenticateAsync(logInRequest.Email, logInRequest.Password);
            if (user == null)
            {
                return Unauthorized(new LogInResponse { IsSuccess = false, Message = "Invalid username or password" });
            }

            if (!string.IsNullOrEmpty(logInRequest.Role) && !await _userService.IsInRoleAsync(user, logInRequest.Role))
            {
                return Unauthorized(new LogInResponse { IsSuccess = false, Message = "User does not have the required role" });
            }

            var userRoles = user.UserRoles.Select(ur => ur.Role.Type).ToList();

            var userData = new
            {
                UserId = user.Id,
                Email = user.Email,
                Username = user.Username,
                Roles = userRoles
            };

            string userDataJson = JsonSerializer.Serialize(userData);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(5),
                HttpOnly = true,
                IsEssential = true,
            };

            Response.Cookies.Append("UserData", userDataJson, cookieOptions);

            return Ok(new LogInResponse { IsSuccess = true, Message = "Login successful", UserDataJson = userDataJson });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            if (string.IsNullOrEmpty(forgotPasswordRequest.Email))
            {
                return BadRequest(new { Message = "Email is required" });
            }

            try
            {
                var (token, createdAt) = await _userService.GeneratePasswordResetTokenWithCreatedAtAsync(forgotPasswordRequest.Email);

                if (token == null)
                {
                    return BadRequest(new { Message = "User not found" });
                }

                var resetLink = Url.Action("ResetPassword", "Auth", new { email = forgotPasswordRequest.Email, token }, Request.Scheme);

                var response = new
                {
                    ResetToken = token,
                    ResetLink = resetLink,
                    CreatedAt = createdAt,
                    Message = "Reset token expires in 5 minutes",
                    Email = forgotPasswordRequest.Email,
                };

                return Ok(response);
            }
            catch (Exception)
            {
               
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            if (string.IsNullOrEmpty(resetPasswordRequest.Email) || string.IsNullOrEmpty(resetPasswordRequest.Token) || string.IsNullOrEmpty(resetPasswordRequest.NewPassword))
            {
                return BadRequest(new { Message = "Email, reset token, and new password are required" });
            }

            var result = await _userService.ResetPasswordAsync(resetPasswordRequest.Email, resetPasswordRequest.Token, resetPasswordRequest.NewPassword, resetPasswordRequest.OldPassword);
            if (result)
            {
                return Ok(new { Message = "Password reset successfully" });
            }

            return BadRequest(new { Message = "Failed to reset password" });
        }
    }
}
