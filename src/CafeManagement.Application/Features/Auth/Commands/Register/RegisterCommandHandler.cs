using AutoMapper;
using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Application.DTOs.Auth;
using CafeManagement.Domain.Entities;
using CafeManagement.Domain.Enums;
using CafeManagement.Domain.Exceptions;
using CafeManagement.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Features.Auth.Commands.Register;

/// <summary>
/// Handler xử lý đăng ký user mới
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, TokenDto>
{
    private readonly IApplicationDbContext _context;
    private readonly PasswordHasher _passwordHasher;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;

    public RegisterCommandHandler(
        IApplicationDbContext context,
        PasswordHasher passwordHasher,
        JwtTokenService jwtTokenService,
        IMapper mapper)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
    }

    /// <summary>
    /// Xử lý đăng ký theo các bước:
    /// 1. Kiểm tra username/email đã tồn tại chưa
    /// 2. Hash password
    /// 3. Tạo User entity
    /// 4. Lưu vào database
    /// 5. Generate JWT token
    /// 6. Return token + user info
    /// </summary>
    public async Task<TokenDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // ===== BƯỚC 1: Kiểm tra username đã tồn tại =====
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email, 
                cancellationToken);

        if (existingUser != null)
        {
            if (existingUser.Username == request.Username)
            {
                throw new DomainException("Username đã tồn tại");
            }
            throw new DomainException("Email đã tồn tại");
        }

        // ===== BƯỚC 2: Hash password =====
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // ===== BƯỚC 3: Tạo User entity =====
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Role = UserRole.Staff,  // Mặc định là Staff
            IsActive = true
        };

        // ===== BƯỚC 4: Lưu vào database =====
        _context.Users.Add(user);
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
