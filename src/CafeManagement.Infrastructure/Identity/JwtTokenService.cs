using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CafeManagement.Infrastructure.Identity;

/// <summary>
/// Service để generate JWT tokens
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Generate JWT token cho user
    /// Token chứa: UserId, Username, Email, Role
    /// </summary>
    public string GenerateToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // ===== CLAIMS =====
        // Claims là thông tin được embed trong JWT token
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),        // Subject (UserId)
            new Claim(JwtRegisteredClaimNames.Name, user.Username),            // Username
            new Claim(JwtRegisteredClaimNames.Email, user.Email),              // Email
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),          // NameIdentifier
            new Claim(ClaimTypes.Name, user.Username),                         // Name
            new Claim(ClaimTypes.Email, user.Email),                           // Email
            new Claim(ClaimTypes.Role, user.Role.ToString()),                  // Role
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID (unique)
            new Claim("fullName", user.FullName)                               // Custom claim
        };

        // ===== TOKEN DESCRIPTOR =====
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(
                int.Parse(jwtSettings["ExpiryMinutes"] ?? "60")),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = credentials
        };

        // ===== GENERATE TOKEN =====
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Validate JWT token
    /// </summary>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
