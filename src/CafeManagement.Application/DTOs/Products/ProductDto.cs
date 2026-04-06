using CafeManagement.Domain.Enums;

namespace CafeManagement.Application.DTOs.Products;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public ProductCategory Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public string? ImageUrl { get; set; }
    public List<ProductIngredientDto> Ingredients { get; set; } = new();
}

public class ProductIngredientDto
{
    public Guid IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal QuantityRequired { get; set; }
    public string Unit { get; set; } = string.Empty;
}
