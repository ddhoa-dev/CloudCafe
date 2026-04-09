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

            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
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
