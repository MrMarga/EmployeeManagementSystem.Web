using System.Security.Cryptography;

namespace backend_app
{
    public class JwtSecretKeyGenerator
    {
        public static string GenerateJwtSecretKey(int length = 32)
        {
            byte[] keyBytes = new byte[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }

            return BitConverter.ToString(keyBytes).Replace("-", "").ToLower();
        }
    }
}
