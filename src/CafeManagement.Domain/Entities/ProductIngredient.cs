using CafeManagement.Domain.Common;

namespace CafeManagement.Domain.Entities;

/// <summary>
/// Công thức: Mối quan hệ giữa Product và Ingredient (Recipe)
/// Ví dụ: Cafe Sữa = 20g Cafe + 100ml Sữa + 10g Đường
/// </summary>
public class ProductIngredient : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    
    public decimal QuantityRequired { get; set; } // Số lượng nguyên liệu cần cho 1 sản phẩm
}
