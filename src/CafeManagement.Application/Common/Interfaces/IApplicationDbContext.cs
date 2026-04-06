using CafeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Common.Interfaces;

/// <summary>
/// Interface cho DbContext - Application layer không phụ thuộc trực tiếp vào EF Core implementation
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Ingredient> Ingredients { get; }
    DbSet<ProductIngredient> ProductIngredients { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<User> Users { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
