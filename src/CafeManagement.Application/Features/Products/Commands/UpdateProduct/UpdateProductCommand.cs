using CafeManagement.Application.DTOs.Products;
using CafeManagement.Domain.Enums;
using MediatR;

namespace CafeManagement.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// Command để cập nhật thông tin sản phẩm và công thức
/// </summary>
public class UpdateProductCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public ProductCategory Category { get; set; }
    public bool IsAvailable { get; set; }
    public string? ImageUrl { get; set; }
    public List<CreateProductIngredientDto> Ingredients { get; set; } = new();
}
