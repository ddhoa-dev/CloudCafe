using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Domain.Enums;
using CafeManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public CancelOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ThenInclude(p => p.ProductIngredients)
            .ThenInclude(pi => pi.Ingredient)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
        {
            throw new NotFoundException("Order", request.OrderId);
        }

        // Chỉ cho phép hủy đơn ở trạng thái Pending hoặc Preparing
        if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Preparing)
        {
            throw new DomainException($"Không thể hủy đơn hàng ở trạng thái {order.Status}");
        }

        // Hoàn trả nguyên liệu vào kho
        await RestoreInventoryAsync(order, cancellationToken);

        // Cập nhật trạng thái
        order.Status = OrderStatus.Cancelled;
        if (!string.IsNullOrEmpty(request.CancellationReason))
        {
            order.Notes = $"{order.Notes}\n[Hủy đơn] {request.CancellationReason}";
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    /// <summary>
    /// Hoàn trả nguyên liệu vào kho khi hủy đơn
    /// </summary>
    private async Task RestoreInventoryAsync(Domain.Entities.Order order, CancellationToken cancellationToken)
    {
        var ingredientRestores = new Dictionary<Guid, decimal>();

        // Tính tổng nguyên liệu cần hoàn trả
        foreach (var orderItem in order.OrderItems)
        {
            foreach (var productIngredient in orderItem.Product.ProductIngredients)
            {
                var totalToRestore = productIngredient.QuantityRequired * orderItem.Quantity;

                if (ingredientRestores.ContainsKey(productIngredient.IngredientId))
                {
                    ingredientRestores[productIngredient.IngredientId] += totalToRestore;
                }
                else
                {
                    ingredientRestores[productIngredient.IngredientId] = totalToRestore;
                }
            }
        }

        // Cập nhật tồn kho
        foreach (var (ingredientId, quantityToRestore) in ingredientRestores)
        {
            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Id == ingredientId, cancellationToken);

            if (ingredient != null)
            {
                ingredient.QuantityInStock += quantityToRestore;
            }
        }
    }
}
