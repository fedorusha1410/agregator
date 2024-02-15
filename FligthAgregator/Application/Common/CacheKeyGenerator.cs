using System.Security.Cryptography;
using System.Text;

namespace Application.Common;

public class CacheKeyGenerator
{
    public static string GenerateCacheKey(string from, string to, string time)
    {
        var combinedInput = from + to + time;

        using (var sha256Hash = SHA256.Create())
        {
            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(combinedInput));
            var builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}