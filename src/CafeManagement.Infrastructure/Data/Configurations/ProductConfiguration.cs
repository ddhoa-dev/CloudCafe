using CafeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Configuration cho Product
/// Định nghĩa schema, constraints, relationships
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // ===== TABLE NAME =====
        builder.ToTable("Products");

        // ===== PRIMARY KEY =====
        builder.HasKey(p => p.Id);

        // ===== PROPERTIES =====
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2)  // decimal(18,2) - 2 chữ số thập phân
            .IsRequired();

        builder.Property(p => p.Category)
            .IsRequired()
            .HasConversion<int>();  // Lưu Enum dưới dạng int

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);

        builder.Property(p => p.IsAvailable)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // ===== AUDITING FIELDS =====
        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.CreatedBy)
            .HasMaxLength(100);

        builder.Property(p => p.LastModifiedAt);

        builder.Property(p => p.LastModifiedBy)
            .HasMaxLength(100);

        // ===== RELATIONSHIPS =====
        // Product -> ProductIngredients (One-to-Many)
        builder.HasMany(p => p.ProductIngredients)
            .WithOne(pi => pi.Product)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);  // Xóa Product → Xóa ProductIngredients

        // Product -> OrderItems (One-to-Many)
        builder.HasMany(p => p.OrderItems)
            .WithOne(oi => oi.Product)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);  // Không cho xóa Product nếu có OrderItems

        // ===== INDEXES =====
        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.IsAvailable);
    }
}
