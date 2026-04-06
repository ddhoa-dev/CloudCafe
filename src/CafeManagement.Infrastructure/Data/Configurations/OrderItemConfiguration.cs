using CafeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Configuration cho OrderItem
/// </summary>
public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(oi => oi.Id);

        // ===== PROPERTIES =====
        builder.Property(oi => oi.Quantity)
            .IsRequired();

        builder.Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(oi => oi.TotalPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(oi => oi.Notes)
            .HasMaxLength(500);

        builder.Property(oi => oi.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // ===== AUDITING FIELDS =====
        builder.Property(oi => oi.CreatedAt).IsRequired();
        builder.Property(oi => oi.CreatedBy).HasMaxLength(100);
        builder.Property(oi => oi.LastModifiedAt);
        builder.Property(oi => oi.LastModifiedBy).HasMaxLength(100);

        // ===== RELATIONSHIPS =====
        // Đã được define ở OrderConfiguration và ProductConfiguration

        // ===== INDEXES =====
        builder.HasIndex(oi => oi.OrderId);
        builder.HasIndex(oi => oi.ProductId);
    }
}
