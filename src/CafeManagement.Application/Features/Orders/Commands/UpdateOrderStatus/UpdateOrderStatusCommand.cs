using CafeManagement.Domain.Enums;
using MediatR;

namespace CafeManagement.Application.Features.Orders.Commands.UpdateOrderStatus;

/// <summary>
/// Command để cập nhật trạng thái đơn hàng
/// </summary>
public class UpdateOrderStatusCommand : IRequest<Unit>
{
    public Guid OrderId { get; set; }
    public OrderStatus NewStatus { get; set; }
}
