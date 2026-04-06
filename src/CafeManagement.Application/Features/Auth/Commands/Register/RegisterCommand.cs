using CafeManagement.Application.DTOs.Auth;
using MediatR;

namespace CafeManagement.Application.Features.Auth.Commands.Register;

/// <summary>
/// Command để đăng ký user mới
/// </summary>
public class RegisterCommand : IRequest<TokenDto>
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}
