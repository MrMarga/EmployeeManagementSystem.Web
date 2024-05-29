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

           
            var createdAt = DateTime.UtcNow;

           
            var tokenExpirationTime = DateTime.UtcNow.AddMinutes(5);

            var tokenWithExpiration = $"{tokenWithSignature}:{tokenExpirationTime:O}";

            var tokenEntity = new Tokens
            {
                Value = tokenWithExpiration,
                CreatedAt = createdAt,
                ExpirationTime = tokenExpirationTime,
                UserId = user.Id // Assuming UserId is the foreign key in Token entity
            };

            _context.Tokens.Add(tokenEntity);
            await _context.SaveChangesAsync();

            return (tokenWithExpiration, createdAt);
        }
        catch (Exception ex)
        {
            // Log or handle the exception
            return (null, DateTime.MinValue); // Return null if an error occurs
        }
    }


    public async Task<bool> ResetPasswordAsync(string email, string tokenWithExpiration, string newPassword, string oldPassword)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return false; // User with the given email does not exist
        }

        if (user.PasswordHash != ComputeHash(oldPassword))
        {
            return false; // Old password doesn't match
        }

        try
        {
            var parts = tokenWithExpiration.Split(':', 3); 
            if (parts.Length != 3)
            {
                return false; 
            }

            var receivedToken = parts[0];
            var receivedSignature = parts[1];
            var receivedExpirationTime = parts[2];

            var tokenEntity = await _context.Tokens.SingleOrDefaultAsync(t => t.Value == tokenWithExpiration);
            if (tokenEntity == null)
            {
                return false; // Token not found in the database
            }

            if (tokenEntity.ExpirationTime < DateTime.UtcNow)
            {
                return false; // Token has expired
            }

            var computedSignature = ComputeHash(user.Id.ToString());
            if (computedSignature != receivedSignature)
            {
                return false; // Token signature does not match
            }

            user.PasswordHash = ComputeHash(newPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            // Log or handle the exception
            return false;
        }
    }


}