// Services/UserService.cs
using backend_app.Data;
using backend_app.Model;
using backend_app.UserRepository;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;


public class UserService : IUserServices
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateUserAsync(string name, string username, string email, string password, string roleName)
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
            PasswordHash = ComputeHash(password)
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

    public string ComputeHash(string value)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        return ComputeHash(providedPassword) == hashedPassword;
    }

    public async Task<(string token, DateTime createdAt)> GeneratePasswordResetTokenWithCreatedAtAsync(string email)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return (null, DateTime.MinValue);
        }

        try
        {
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var signature = ComputeHash(user.Id.ToString());

            var tokenWithSignature = $"{token}:{signature}";
            var createdAt = DateTime.Now;

            
            var tokenExpirationTime = DateTime.Now.AddMinutes(5);

            var tokenWithExpiration = $"{tokenWithSignature}:{tokenExpirationTime:O}";

            return (tokenWithExpiration, createdAt); 
        }
        catch (Exception ex)
        {
            // Log or handle the exception
            return (null, DateTime.MinValue); // Return null if an error occurs
        }
    }

    public async Task<bool> ResetPasswordAsync(string email, string tokenWithExpiration, string newPassword)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return false; // User with the given email does not exist
        }

        try
        {
            var parts = tokenWithExpiration.Split(':', 3); // Ensure we split into exactly 3 parts
            if (parts.Length != 3)
            {
                return false; // Token format is invalid
            }

            var receivedToken = parts[0];
            var receivedId = parts[1];
            var emailHash = ComputeHash(user.Id.ToString()); // Compute hash using user ID
            var tokenExpirationString = parts[2]; // Corrected index

            // Parse the token expiration time using the "o" (round-trip) format specifier
            if (!DateTime.TryParseExact(tokenExpirationString, "O", null, DateTimeStyles.RoundtripKind, out var tokenExpirationTime))
            {
                return false; // Invalid expiration time format
            }

            if (tokenExpirationTime < DateTime.Now)
            {
                return false; 
            }

            if (receivedId != ComputeHash(user.Id.ToString()))
            {
                return false; 
            }

            
            user.PasswordHash = ComputeHash(newPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            
            return false;
        }
    }



}