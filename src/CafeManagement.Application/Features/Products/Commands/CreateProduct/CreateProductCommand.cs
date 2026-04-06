using CafeManagement.Application.DTOs.Products;
using CafeManagement.Domain.Enums;
using MediatR;

namespace CafeManagement.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Command để tạo sản phẩm mới kèm công thức (Recipe)
/// Ví dụ: Tạo "Cafe Sữa" với công thức: 20g Cafe + 100ml Sữa + 10g Đường
/// </summary>
public class CreateProductCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public ProductCategory Category { get; set; }
    public string? ImageUrl { get; set; }
    public List<CreateProductIngredientDto> Ingredients { get; set; } = new();
}
