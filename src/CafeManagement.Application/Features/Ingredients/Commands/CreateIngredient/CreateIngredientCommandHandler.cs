using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Domain.Entities;
using MediatR;

namespace CafeManagement.Application.Features.Ingredients.Commands.CreateIngredient;

/// <summary>
/// Handler xử lý tạo nguyên liệu mới
/// </summary>
public class CreateIngredientCommandHandler : IRequestHandler<CreateIngredientCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateIngredientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateIngredientCommand request, CancellationToken cancellationToken)
    {
        var ingredient = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Unit = request.Unit,
            QuantityInStock = request.QuantityInStock,
            MinimumStockLevel = request.MinimumStockLevel,
            UnitPrice = request.UnitPrice
        };

        _context.Ingredients.Add(ingredient);
        await _context.SaveChangesAsync(cancellationToken);

        return ingredient.Id;
    }
}
