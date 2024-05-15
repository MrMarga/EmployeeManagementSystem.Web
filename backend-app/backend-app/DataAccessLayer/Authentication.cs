using backend_app.Model;
using Npgsql;
using System.Data.Common;


namespace backend_app.DataAccessLayer
{
    public class Authentication : IAuthentication
    {
        private readonly IConfiguration _configuration;
        private readonly NpgsqlConnection _npgSqlConnection;

        public Authentication(IConfiguration configuration)
        {
            _configuration = configuration;
            _npgSqlConnection = new NpgsqlConnection(_configuration["ConnectionStrings:Backend-DB"]);
        }

        public async Task<SignUpResponse> SignUp(SignUpRequest request)
        {
            SignUpResponse response = new SignUpResponse
            {
                IsSuccess = true,
                Message = "Successful"
            };

            try
            {
                if (!request.Password.Equals(request.ConfirmPassword))
                {
                    response.IsSuccess = false;
                    response.Message = "Password Not Matched.";
                    return response;
                }

                if (_npgSqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _npgSqlConnection.OpenAsync();
                }

                string npgsqlQuery = @"INSERT INTO public.""UserDetails""(""Name"",""Username"",""Password"",""Email"",""Role"") VALUES(@Name,@Username,@Password,@Email,@Role)";

                using (NpgsqlCommand npgsqlcmd = new NpgsqlCommand(npgsqlQuery, _npgSqlConnection))
                {
                    npgsqlcmd.CommandType = System.Data.CommandType.Text;
                    npgsqlcmd.CommandTimeout = 180;
                    npgsqlcmd.Parameters.AddWithValue(parameterName: "@Name", request.Name);
                    npgsqlcmd.Parameters.AddWithValue(parameterName: "@Username", request.Username);
                    npgsqlcmd.Parameters.AddWithValue(parameterName: "@Email", request.Email.ToLower());
                    npgsqlcmd.Parameters.AddWithValue(parameterName: "@Password", request.Password);
                    npgsqlcmd.Parameters.AddWithValue(parameterName: "@Role", request.Role);
                    

                    int status = await npgsqlcmd.ExecuteNonQueryAsync();
                    if (status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Something went wrong";
                    }
                }
            }
            catch (Exception ex)
            { 
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
                await _npgSqlConnection.CloseAsync();
                await _npgSqlConnection.DisposeAsync();
            }
            return response;
        }

        public async Task<LogInResponse> Login(LogInRequest request)
        {
            LogInResponse response = new LogInResponse();

            try
            {
                await using (_npgSqlConnection)
                {
                    if (_npgSqlConnection.State != System.Data.ConnectionState.Open)
                    {
                        await _npgSqlConnection.OpenAsync();
                    }

                    string npgsqlQuery = @"SELECT * FROM public.""UserDetails"" WHERE ""Email"" = @Email AND ""Password"" = @Password AND ""Role"" = @Role";


                    await using (NpgsqlCommand npgsqlcmd = new NpgsqlCommand(npgsqlQuery, _npgSqlConnection))
                    {
                        npgsqlcmd.CommandType = System.Data.CommandType.Text;
                        npgsqlcmd.CommandTimeout = 180;
                        npgsqlcmd.Parameters.AddWithValue(parameterName: "@Email", request.Email);
                        npgsqlcmd.Parameters.AddWithValue(parameterName: "@Password", request.Password);
                        npgsqlcmd.Parameters.AddWithValue(parameterName: "@Role", request.Role);


                        using (DbDataReader dataReader = await npgsqlcmd.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                response.IsSuccess = true;
                                response.Message = "Logged In Successfully";
                            }
                            else
                            {
                                response.Message = "Login Failed.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

    }
}
