namespace CafeManagement.Application.DTOs.Ingredients;

public class IngredientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal QuantityInStock { get; set; }
    public decimal MinimumStockLevel { get; set; }
    public decimal UnitPrice { get; set; }
    public bool IsLowStock => QuantityInStock <= MinimumStockLevel;
}
