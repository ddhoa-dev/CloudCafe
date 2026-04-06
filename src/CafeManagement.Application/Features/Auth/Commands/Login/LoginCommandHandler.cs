using AutoMapper;
using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Application.DTOs.Auth;
using CafeManagement.Domain.Exceptions;
using CafeManagement.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Features.Auth.Commands.Login;

/// <summary>
/// Handler xử lý đăng nhập
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenDto>
{
    private readonly IApplicationDbContext _context;
    private readonly PasswordHasher _passwordHasher;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;
    private readonly IDateTime _dateTime;

    public LoginCommandHandler(
        IApplicationDbContext context,
        PasswordHasher passwordHasher,
        JwtTokenService jwtTokenService,
        IMapper mapper,
        IDateTime dateTime)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
        _dateTime = dateTime;
    }

    /// <summary>
    /// Xử lý login theo các bước:
    /// 1. Tìm user theo username
    /// 2. Verify password
    /// 3. Kiểm tra user active
    /// 4. Update LastLoginAt
    /// 5. Generate JWT token
    /// 6. Return token + user info
    /// </summary>
    public async Task<TokenDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // ===== BƯỚC 1: Tìm user theo username =====
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

        if (user == null)
        {
            throw new DomainException("Username hoặc password không đúng");
        }

        // ===== BƯỚC 2: Verify password =====
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new DomainException("Username hoặc password không đúng");
        }

        // ===== BƯỚC 3: Kiểm tra user active =====
        if (!user.IsActive)
        {
            throw new DomainException("Tài khoản đã bị vô hiệu hóa");
        }

        // ===== BƯỚC 4: Update LastLoginAt =====
        user.LastLoginAt = _dateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);

        // ===== BƯỚC 5: Generate JWT token =====
        var token = _jwtTokenService.GenerateToken(user);

        // ===== BƯỚC 6: Return TokenDto =====
        return new TokenDto
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresIn = 3600,  // 1 hour
            User = _mapper.Map<UserInfoDto>(user)
        };
    }
}
