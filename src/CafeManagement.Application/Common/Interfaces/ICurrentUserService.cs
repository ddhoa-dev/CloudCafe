namespace CafeManagement.Application.Common.Interfaces;

/// <summary>
/// Service để lấy thông tin user hiện tại từ JWT token
/// </summary>
public interface ICurrentUserService
{
    string? UserId { get; }
    string? Username { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}
