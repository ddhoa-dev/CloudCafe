using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Features.Ingredients.Commands.UpdateIngredient;

/// <summary>
/// Handler xử lý cập nhật nguyên liệu
/// </summary>
public class UpdateIngredientCommandHandler : IRequestHandler<UpdateIngredientCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateIngredientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateIngredientCommand request, CancellationToken cancellationToken)
    {
        var ingredient = await _context.Ingredients
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (ingredient == null)
        {
            throw new NotFoundException("Ingredient", request.Id);
        }

        // Cập nhật thông tin
        ingredient.Name = request.Name;
        ingredient.Description = request.Description;
        ingredient.Unit = request.Unit;
        ingredient.QuantityInStock = request.QuantityInStock;
        ingredient.MinimumStockLevel = request.MinimumStockLevel;
        ingredient.UnitPrice = request.UnitPrice;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
