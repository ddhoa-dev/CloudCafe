using CafeManagement.Domain.Common;
using CafeManagement.Domain.Enums;

namespace CafeManagement.Domain.Entities;

/// <summary>
/// Sản phẩm (Cafe Sữa, Cafe Đen, Trà Sữa, etc.)
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public ProductCategory Category { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? ImageUrl { get; set; }

    // Navigation properties
    public ICollection<ProductIngredient> ProductIngredients { get; set; } = new List<ProductIngredient>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
