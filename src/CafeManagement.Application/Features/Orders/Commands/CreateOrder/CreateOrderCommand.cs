using CafeManagement.Application.DTOs.Orders;
using MediatR;

namespace CafeManagement.Application.Features.Orders.Commands.CreateOrder;

/// <summary>
/// Command để tạo đơn hàng mới
/// Tự động tính toán và trừ nguyên liệu trong kho
/// </summary>
public class CreateOrderCommand : IRequest<Guid>
{
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? Notes { get; set; }
    public decimal? DiscountAmount { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}
