// Services/UserService.cs
using backend_app.Data;
using backend_app.Model;
using backend_app.UserRepository;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


public class UserService : IUserServices
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateUserAsync(string name , string username, string email, string password, string roleName)
    {
        if (await _context.Users.AnyAsync(u => u.Email == email))
        {
            return false;
        }

        var role = await _context.Roles.SingleOrDefaultAsync(r => r.Type == roleName);
        if (role == null)
        {
            role = new Role { Type = roleName };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
        }

        var user = new User
        {
            Name = name,
            Username = username,
            Email = email,
            PasswordHash = HashPassword(password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        };

        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<User> AuthenticateAsync(string email, string password)
    {
        var user = await _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).SingleOrDefaultAsync(u => u.Email == email);
        if (user == null || !VerifyPassword(user.PasswordHash, password))
        {
            return null;
        }

        return user;
    }

    public async Task<bool> IsInRoleAsync(User user, string roleName)
    {
        return user.UserRoles.Any(ur => ur.Role.Type == roleName);
    }

    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        return HashPassword(providedPassword) == hashedPassword;
    }

    public async Task<string> GeneratePasswordResetTokenAsync(string email)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return null;
        }

        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        return token;
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return false;
        }

        user.PasswordHash = HashPassword(newPassword);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return true;
    }
}
