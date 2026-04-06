using MediatR;

namespace CafeManagement.Application.Features.Ingredients.Commands.UpdateIngredient;

/// <summary>
/// Command để cập nhật thông tin nguyên liệu
/// </summary>
public class UpdateIngredientCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal QuantityInStock { get; set; }
    public decimal MinimumStockLevel { get; set; }
    public decimal UnitPrice { get; set; }
}
