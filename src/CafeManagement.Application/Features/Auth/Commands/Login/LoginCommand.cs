using CafeManagement.Application.DTOs.Auth;
using MediatR;

namespace CafeManagement.Application.Features.Auth.Commands.Login;

/// <summary>
/// Command để đăng nhập
/// </summary>
public class LoginCommand : IRequest<TokenDto>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
