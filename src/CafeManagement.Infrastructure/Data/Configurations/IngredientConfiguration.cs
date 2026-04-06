using CafeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Configuration cho Ingredient
/// </summary>
public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("Ingredients");

        builder.HasKey(i => i.Id);

        // ===== PROPERTIES =====
        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Description)
            .HasMaxLength(1000);

        builder.Property(i => i.Unit)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.QuantityInStock)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.MinimumStockLevel)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // ===== AUDITING FIELDS =====
        builder.Property(i => i.CreatedAt).IsRequired();
        builder.Property(i => i.CreatedBy).HasMaxLength(100);
        builder.Property(i => i.LastModifiedAt);
        builder.Property(i => i.LastModifiedBy).HasMaxLength(100);

        // ===== RELATIONSHIPS =====
        builder.HasMany(i => i.ProductIngredients)
            .WithOne(pi => pi.Ingredient)
            .HasForeignKey(pi => pi.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);  // Không cho xóa Ingredient nếu đang dùng trong Product

        // ===== INDEXES =====
        builder.HasIndex(i => i.Name);
        builder.HasIndex(i => i.QuantityInStock);
    }
}
