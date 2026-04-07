namespace CafeManagement.Application.Common.Interfaces;

/// <summary>
/// Interface cho Password Hashing service
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
