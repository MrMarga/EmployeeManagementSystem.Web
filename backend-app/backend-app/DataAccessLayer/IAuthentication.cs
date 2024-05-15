using backend_app.Model;

namespace backend_app.DataAccessLayer
{
    public interface IAuthentication
    {
       public Task<SignUpResponse> SignUp(SignUpRequest request);

       public Task<LogInResponse> Login(LogInRequest request);
    }

}
