using System.Reflection;
using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Domain.Entities;
using CafeManagement.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Infrastructure.Data;

/// <summary>
/// DbContext chính của ứng dụng
/// Implement IApplicationDbContext để Application layer không phụ thuộc trực tiếp vào EF Core
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly AuditableEntityInterceptor _auditableEntityInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditableEntityInterceptor auditableEntityInterceptor)
        : base(options)
    {
        _auditableEntityInterceptor = auditableEntityInterceptor;
    }

    // DbSets - Các bảng trong database
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<ProductIngredient> ProductIngredients => Set<ProductIngredient>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Configure model và apply configurations
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply tất cả IEntityTypeConfiguration từ assembly hiện tại
        // Tự động scan và apply các file Configuration (ProductConfiguration, OrderConfiguration, etc.)
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    /// <summary>
    /// Configure interceptors
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Add AuditableEntityInterceptor để tự động set CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy
        optionsBuilder.AddInterceptors(_auditableEntityInterceptor);
    }

    /// <summary>
    /// Override SaveChangesAsync để có thể customize logic nếu cần
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
