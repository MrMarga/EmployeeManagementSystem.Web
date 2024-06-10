using backend_app.Data;
using backend_app.Model;
using backend_app.UserRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class UserService : IUserServices
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public UserService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
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

    public string ComputeHash(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        return ComputeHash(providedPassword) == hashedPassword;
    }

    public async Task<AuthTokens> GenerateJwtTokenLogin(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
    };

        foreach (var userRole in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Type));
        }

        claims.Add(new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"]));
        claims.Add(new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"]));

        var accessTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var accessToken = tokenHandler.CreateToken(accessTokenDescriptor);
        var accessTokenString = tokenHandler.WriteToken(accessToken);

        // Generate device ID
        string deviceId = GenerateDeviceId(user);

        // Check if a refresh token already exists for the specific device and user
        var existingRefreshToken = await GetExistingRefreshTokenAsync(user.Id, deviceId);
        string refreshToken;

        if (existingRefreshToken != null && existingRefreshToken.ExpirationDate > DateTime.UtcNow)
        {
            refreshToken = existingRefreshToken.Token;
        }
        else
        {
            // Check if a refresh token already exists for the device ID
            var existingRefreshTokenForDevice = await GetExistingRefreshTokenForDeviceAsync(deviceId);
            if (existingRefreshTokenForDevice != null && existingRefreshTokenForDevice.ExpirationDate > DateTime.UtcNow)
            {
                refreshToken = existingRefreshTokenForDevice.Token;
            }
            else
            {
                refreshToken = GenerateRefreshToken();
                await StoreRefreshToken(refreshToken, user.Id, deviceId, DateTime.UtcNow.AddMinutes(15));
            }
        }

        return new AuthTokens
        {
            AccessToken = accessTokenString,
            DeviceID = deviceId,
            RefreshToken = refreshToken
        };
    }

    private async Task<RefreshToken> GetExistingRefreshTokenAsync(int userId, string deviceId)
    {
        return await _context.RefreshTokens
            .SingleOrDefaultAsync(t => t.UserId == userId && t.DeviceId == deviceId && t.ExpirationDate > DateTime.UtcNow);
    }

    private async Task<RefreshToken> GetExistingRefreshTokenForDeviceAsync(string deviceId)
    {
        return await _context.RefreshTokens
            .SingleOrDefaultAsync(t => t.DeviceId == deviceId && t.ExpirationDate > DateTime.UtcNow);
    }

    private string GenerateDeviceId(User user)
    {
        return $"{user.Id}-{Guid.NewGuid()}";
    } 

    private async Task StoreRefreshToken(string refreshToken, int userId, string deviceId, DateTime expirationDate)
    {
        var existingToken = await _context.RefreshTokens
            .SingleOrDefaultAsync(t => t.UserId == userId && t.DeviceId == deviceId);

        if (existingToken != null)
        {
            existingToken.DeviceId = deviceId;
            existingToken.Token = refreshToken;
            existingToken.CreatedAt = DateTime.UtcNow;
            existingToken.ExpirationDate = expirationDate;
            _context.RefreshTokens.Update(existingToken);
        }
        else
        {
            var tokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = userId,
                DeviceId = deviceId,
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = expirationDate
            };
            _context.RefreshTokens.Add(tokenEntity);
        }

        await _context.SaveChangesAsync();
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<AuthTokens> RefreshTokensAsync(string refreshToken, string deviceId)
    {
        // Find the stored refresh token
        var storedToken = await _context.RefreshTokens
            .SingleOrDefaultAsync(t => t.Token == refreshToken && t.DeviceId == deviceId);

        // Check if the refresh token exists and is not expired
        if (storedToken == null || storedToken.ExpirationDate < DateTime.UtcNow)
        {
            return null;
        }

        // Find the user associated with the refresh token
        var user = await _context.Users.Include(u => u.UserRoles)
                                       .ThenInclude(ur => ur.Role)
                                       .SingleOrDefaultAsync(u => u.Id == storedToken.UserId);

        if (user == null)
        {
            return null;
        }

        // Generate a new access token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
    };

        foreach (var userRole in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Type));
        }

        claims.Add(new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"]));
        claims.Add(new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"]));

        var accessTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var accessToken = tokenHandler.CreateToken(accessTokenDescriptor);
        var accessTokenString = tokenHandler.WriteToken(accessToken);

        // Optionally rotate refresh token here if required
        storedToken.Token = GenerateRefreshToken();
        storedToken.CreatedAt = DateTime.UtcNow;
        storedToken.ExpirationDate = DateTime.UtcNow.AddMinutes(15);
        _context.RefreshTokens.Update(storedToken);
        await _context.SaveChangesAsync();

        // Return the new access token and the new refresh token
        return new AuthTokens
        {
            AccessToken = accessTokenString,
            RefreshToken = storedToken.Token
        };
    }





    public async Task<(string token, string createdAt)> GeneratePasswordResetTokenWithCreatedAtAsync(string email)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return (null, null);
        }

        try
        {
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var signature = ComputeHash(user.Id.ToString());

            var tokenWithSignature = $"{token}:{signature}";

            // Use Nepal time for token creation and expiration
            var createdAt = DateTime.UtcNow.ToLocalTime();
            var tokenExpirationTime = createdAt.AddMinutes(5);

            var createdAtLocalFormatted = createdAt.ToString("MM/dd/yyyy hh:mm:ss tt");
            var tokenExpirationTimeFormatted = tokenExpirationTime.ToString("MM/dd/yyyy hh:mm:ss tt");

            var tokenWithExpiration = $"{tokenWithSignature}:{tokenExpirationTimeFormatted}";

            var tokenEntity = new Tokens
            {
                Value = tokenWithExpiration,
                CreatedAt = createdAt.ToUniversalTime(), // Convert to UTC before saving
                ExpirationTime = tokenExpirationTime.ToUniversalTime(), // Convert to UTC before saving
                UserId = user.Id
            };

            _context.Tokens.Add(tokenEntity);
            await _context.SaveChangesAsync();

            // Return the token and the local creation time in 12-hour format
            return (tokenWithExpiration, createdAtLocalFormatted);
        }
        catch (Exception)
        {
            return (null, null);
        }
    }

    public async Task<bool> ResetPasswordAsync(string email, string tokenWithExpiration, string newPassword, string oldPassword)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return false;
        }

        if (user.PasswordHash != ComputeHash(oldPassword))
        {
            return false;
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
                return false;
            }

            var tokenExpirationTime = DateTime.ParseExact(receivedExpirationTime, "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            if (tokenExpirationTime < DateTime.UtcNow)
            {
                return false; // Token has expired
            }

            var computedSignature = ComputeHash(user.Id.ToString());
            if (computedSignature != receivedSignature)
            {
                return false; // Token signature does not match
            }

            // If all checks pass, update the password
            user.PasswordHash = ComputeHash(newPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception)
        {
            // Log or handle the exception
            return false;
        }
    }
}
