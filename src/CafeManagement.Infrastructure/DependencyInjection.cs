using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Infrastructure.Data;
using CafeManagement.Infrastructure.Data.Interceptors;
using CafeManagement.Infrastructure.Identity;
using CafeManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CafeManagement.Infrastructure;

/// <summary>
/// Extension methods để register Infrastructure services vào DI container
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ===== DATABASE =====
        // Register AuditableEntityInterceptor
        services.AddScoped<AuditableEntityInterceptor>();

        // Register DbContext với PostgreSQL
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditableEntityInterceptor>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Tự động chuyển đổi Render Internal Database URL (postgres:// hoặc postgresql://) sang chuẩn của .NET Npgsql
            if (!string.IsNullOrEmpty(connectionString))
            {
                connectionString = connectionString.Trim().Trim('"'); // Xóa khoảng trắng và dấu ngoặc kép thừa
                if (connectionString.StartsWith("postgres://") || connectionString.StartsWith("postgresql://"))
                {
                    var databaseUri = new Uri(connectionString);
                    var userInfo = databaseUri.UserInfo.Split(':');
                    connectionString = $"Host={databaseUri.Host};Port={(databaseUri.Port > 0 ? databaseUri.Port : 5432)};Database={databaseUri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SslMode=Prefer;TrustServerCertificate=true;";
                }
            }

            options.UseNpgsql(
                connectionString,
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
        });

        // Register IApplicationDbContext
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // ===== SERVICES =====
        services.AddScoped<IDateTime, DateTimeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // ===== IDENTITY SERVICES =====
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
