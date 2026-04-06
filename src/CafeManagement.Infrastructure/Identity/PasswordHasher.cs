using System.Security.Cryptography;
using System.Text;

namespace CafeManagement.Infrastructure.Identity;

/// <summary>
/// Service để hash và verify passwords
/// Sử dụng SHA256 (trong production nên dùng BCrypt hoặc Argon2)
/// </summary>
public class PasswordHasher
{
    /// <summary>
    /// Hash password với SHA256
    /// NOTE: Trong production, nên dùng BCrypt, Argon2, hoặc PBKDF2
    /// </summary>
    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    /// <summary>
    /// Verify password với hash
    /// </summary>
    public bool VerifyPassword(string password, string hash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == hash;
    }
}
