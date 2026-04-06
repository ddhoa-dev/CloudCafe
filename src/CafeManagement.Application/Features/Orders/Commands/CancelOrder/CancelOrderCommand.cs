using MediatR;

namespace CafeManagement.Application.Features.Orders.Commands.CancelOrder;

/// <summary>
/// Command để hủy đơn hàng và hoàn trả nguyên liệu vào kho
/// </summary>
public class CancelOrderCommand : IRequest<Unit>
{
    public Guid OrderId { get; set; }
    public string? CancellationReason { get; set; }
}
