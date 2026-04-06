using CafeManagement.Domain.Common;

namespace CafeManagement.Domain.Entities;

/// <summary>
/// Nguyên liệu (Cafe, Sữa, Đường, etc.)
/// </summary>
public class Ingredient : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Unit { get; set; } = string.Empty; // gram, ml, kg, etc.
    public decimal QuantityInStock { get; set; }
    public decimal MinimumStockLevel { get; set; } // Ngưỡng cảnh báo hết hàng
    public decimal UnitPrice { get; set; } // Giá mua vào
    
    // Navigation properties
    public ICollection<ProductIngredient> ProductIngredients { get; set; } = new List<ProductIngredient>();
}
