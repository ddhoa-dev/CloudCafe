using System.Security.Claims;
using CafeManagement.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CafeManagement.Infrastructure.Services;

/// <summary>
/// Implementation của ICurrentUserService
/// Lấy thông tin user hiện tại từ JWT token trong HttpContext
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Lấy UserId từ JWT token claim "sub" hoặc "nameid"
    /// </summary>
    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                            ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");

    /// <summary>
    /// Lấy Username từ JWT token claim "name"
    /// </summary>
    public string? Username => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name)
                              ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("name");

    /// <summary>
    /// Lấy Email từ JWT token claim "email"
    /// </summary>
    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email)
                           ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("email");

    /// <summary>
    /// Kiểm tra user đã authenticated chưa
    /// </summary>
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
