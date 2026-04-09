using CafeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Configuration cho Order
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        // ===== PROPERTIES =====
        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.OrderDate)
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<int>();  // Lưu Enum dưới dạng int

        builder.Property(o => o.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(o => o.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(o => o.FinalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(o => o.CustomerName)
            .HasMaxLength(200);

        builder.Property(o => o.CustomerPhone)
            .HasMaxLength(20);

        builder.Property(o => o.Notes)
            .HasMaxLength(1000);

        builder.Property(o => o.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // ===== AUDITING FIELDS =====
        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.CreatedBy).HasMaxLength(100);
        builder.Property(o => o.LastModifiedAt);
        builder.Property(o => o.LastModifiedBy).HasMaxLength(100);

        // ===== RELATIONSHIPS =====
        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);  // Xóa Order → Xóa OrderItems

        // ===== INDEXES =====
        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();  // OrderNumber phải unique

        builder.HasIndex(o => o.OrderDate);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CustomerPhone);
    }
}
