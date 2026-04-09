using CafeManagement.Domain.Common;
using CafeManagement.Domain.Enums;

namespace CafeManagement.Domain.Entities;

/// <summary>
/// Đơn hàng
/// </summary>
public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty; // Mã đơn hàng tự động
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
