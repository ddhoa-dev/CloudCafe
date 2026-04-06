using CafeManagement.Domain.Enums;

namespace CafeManagement.Application.DTOs.Products;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public ProductCategory Category { get; set; }
    public string? ImageUrl { get; set; }
    public List<CreateProductIngredientDto> Ingredients { get; set; } = new();
}

public class CreateProductIngredientDto
{
    public Guid IngredientId { get; set; }
    public decimal QuantityRequired { get; set; }
}
