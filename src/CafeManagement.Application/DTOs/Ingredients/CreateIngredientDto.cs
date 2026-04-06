namespace CafeManagement.Application.DTOs.Ingredients;

public class CreateIngredientDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal QuantityInStock { get; set; }
    public decimal MinimumStockLevel { get; set; }
    public decimal UnitPrice { get; set; }
}
