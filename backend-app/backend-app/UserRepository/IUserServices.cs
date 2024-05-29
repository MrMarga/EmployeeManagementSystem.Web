using backend_app.Model;

namespace backend_app.UserRepository
{
    public interface IUserServices
    {
        Task<bool> CreateUserAsync(string name ,string username, string email, string password, string roleName);
        Task<User> AuthenticateAsync(string email, string password);
        Task<bool> IsInRoleAsync(User user, string roleName);
        string ComputeHash(string value);
        bool VerifyPassword(string hashedPassword, string providedPassword);
        Task<(string token, DateTime createdAt)> GeneratePasswordResetTokenWithCreatedAtAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword, string oldPassword);
    }
}
