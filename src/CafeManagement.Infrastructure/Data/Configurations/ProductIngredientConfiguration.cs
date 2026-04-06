using CafeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Configuration cho ProductIngredient (Công thức)
/// Đây là bảng Many-to-Many giữa Product và Ingredient
/// </summary>
public class ProductIngredientConfiguration : IEntityTypeConfiguration<ProductIngredient>
{
    public void Configure(EntityTypeBuilder<ProductIngredient> builder)
    {
        builder.ToTable("ProductIngredients");

        builder.HasKey(pi => pi.Id);

        // ===== PROPERTIES =====
        builder.Property(pi => pi.QuantityRequired)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(pi => pi.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // ===== AUDITING FIELDS =====
        builder.Property(pi => pi.CreatedAt).IsRequired();
        builder.Property(pi => pi.CreatedBy).HasMaxLength(100);
        builder.Property(pi => pi.LastModifiedAt);
        builder.Property(pi => pi.LastModifiedBy).HasMaxLength(100);

        // ===== RELATIONSHIPS =====
        // Đã được define ở ProductConfiguration và IngredientConfiguration

        // ===== INDEXES =====
        // Composite index cho ProductId + IngredientId để tránh duplicate
        builder.HasIndex(pi => new { pi.ProductId, pi.IngredientId })
            .IsUnique();
    }
}
