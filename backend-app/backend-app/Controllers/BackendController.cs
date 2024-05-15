using backend_app.DataAccessLayer;
using backend_app.Model;
using Microsoft.AspNetCore.Mvc;

namespace backend_app.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class BackendController : ControllerBase
    {

        protected readonly IAuthentication _authentication;

        public BackendController(IAuthentication authentication)
        {
            _authentication = authentication;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpRequest request)
        {
            SignUpResponse response = new();
            try
            {
             response = await _authentication.SignUp(request);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;

            }
            return Ok(response);
        }

        [HttpPost]
        public  async Task<IActionResult> Login(LogInRequest request)
        {
            LogInResponse response = new ();
            try
            {
                response = await _authentication.Login(request);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;

            }
            return Ok(response);
        }
    }
}
