using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using backend_app.Model;

namespace backend_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUpRequest signUpRequest)
        {
            if (signUpRequest.Password != signUpRequest.ConfirmPassword)
            {
                return BadRequest(new SignUpResponse { IsSuccess = false, Message = "Passwords do not match" });
            }

            var user = new IdentityUser { UserName = signUpRequest.Username, Email = signUpRequest.Email };
            var result = await _userManager.CreateAsync(user, signUpRequest.Password);

            if (result.Succeeded)
            {
                // Check if role exists, if not, create it
                if (!await _roleManager.RoleExistsAsync(signUpRequest.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(signUpRequest.Role));
                }

                // Assign the role to the user
                await _userManager.AddToRoleAsync(user, signUpRequest.Role);

                return Ok(new SignUpResponse { IsSuccess = true, Message = "User created successfully" });
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new SignUpResponse { IsSuccess = false, Message = errors });
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn(LogInRequest logInRequest)
        {
            var user = await _userManager.FindByEmailAsync(logInRequest.Email);
            if (user == null)
            {
                return Unauthorized(new LogInResponse { IsSuccess = false, Message = "Invalid username or password" });
            }

            // Attempt to sign in the user
            var result = await _signInManager.PasswordSignInAsync(user, logInRequest.Password, false, false);
            if (result.Succeeded)
            {
                // Check if the user has the requested role
                if (!string.IsNullOrEmpty(logInRequest.Role) && !await _userManager.IsInRoleAsync(user, logInRequest.Role))
                {
                    // If the user doesn't have the requested role, return unauthorized
                    return Unauthorized(new LogInResponse { IsSuccess = false, Message = "User does not have the required role" });
                }

                // Retrieve user roles
                var userRoles = await _userManager.GetRolesAsync(user);

                // Store additional information in userData object
                var userData = new
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Username = user.UserName,
                    Roles = userRoles // Include user roles in the response
                };

                // Serialize userData to JSON string
                string userDataJson = JsonSerializer.Serialize(userData);

                // Return userData along with the response
                return Ok(new LogInResponse { IsSuccess = true, Message = "Login successful", UserDataJson = userDataJson });
            }

            return Unauthorized(new LogInResponse { IsSuccess = false, Message = "Invalid username or password" });
        }

       
    }
}
