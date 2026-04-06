using MediatR;

namespace CafeManagement.Application.Features.Ingredients.Commands.CreateIngredient;

/// <summary>
/// Command để tạo nguyên liệu mới
/// </summary>
public class CreateIngredientCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Unit { get; set; } = string.Empty;  // gram, ml, kg, lít
    public decimal QuantityInStock { get; set; }
    public decimal MinimumStockLevel { get; set; }
    public decimal UnitPrice { get; set; }
}
