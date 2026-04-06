using CafeManagement.Application.Common.Models;
using CafeManagement.Application.DTOs.Orders;
using CafeManagement.Domain.Enums;
using MediatR;

namespace CafeManagement.Application.Features.Orders.Queries.GetOrders;

/// <summary>
/// Query để lấy danh sách đơn hàng với pagination và filter
/// </summary>
public class GetOrdersQuery : IRequest<PaginatedList<OrderDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public OrderStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? CustomerPhone { get; set; }
}
