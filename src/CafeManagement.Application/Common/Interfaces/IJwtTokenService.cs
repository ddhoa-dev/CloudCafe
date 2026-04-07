using CafeManagement.Domain.Entities;

namespace CafeManagement.Application.Common.Interfaces;

/// <summary>
/// Interface cho JWT Token service
/// </summary>
public interface IJwtTokenService
{
    string GenerateToken(User user);
}
