using CafeManagement.Domain.Common;

namespace CafeManagement.Domain.Entities;

/// <summary>
/// Chi tiết đơn hàng
/// </summary>
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; } // Ghi chú đặc biệt (ít đường, nhiều đá, etc.)
}
