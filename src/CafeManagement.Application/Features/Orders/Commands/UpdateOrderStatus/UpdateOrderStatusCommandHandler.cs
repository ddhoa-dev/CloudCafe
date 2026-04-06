using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Features.Orders.Commands.UpdateOrderStatus;

/// <summary>
/// Handler xử lý cập nhật trạng thái đơn hàng
/// Flow: Pending → Preparing → Ready → Completed
/// </summary>
public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateOrderStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Cập nhật trạng thái đơn hàng
    /// Ví dụ:
    /// - Staff nhận đơn: Pending → Preparing
    /// - Pha chế xong: Preparing → Ready
    /// - Giao cho khách: Ready → Completed
    /// </summary>
    public async Task<Unit> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        // Tìm đơn hàng theo ID
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
        {
            throw new NotFoundException("Order", request.OrderId);
        }

        // Cập nhật trạng thái mới
        order.Status = request.NewStatus;
        
        // EF Core sẽ track thay đổi và update khi SaveChanges()
        await _context.SaveChangesAsync(cancellationToken);

        // Unit.Value = void trong MediatR (không return gì)
        return Unit.Value;
    }
}
