using AutoMapper;
using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Application.DTOs.Orders;
using CafeManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Features.Orders.Queries.GetOrderById;

/// <summary>
/// Handler xử lý query lấy chi tiết 1 đơn hàng theo ID
/// </summary>
public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetOrderByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Lấy chi tiết đơn hàng bao gồm:
    /// - Thông tin đơn hàng (Order)
    /// - Chi tiết sản phẩm (OrderItems)
    /// - Thông tin sản phẩm (Product)
    /// </summary>
    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        // Query đơn hàng với Include để load related data
        var order = await _context.Orders
            .Include(o => o.OrderItems)           // Load chi tiết đơn hàng
            .ThenInclude(oi => oi.Product)        // Load thông tin sản phẩm
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        // Throw exception nếu không tìm thấy
        if (order == null)
        {
            throw new NotFoundException("Order", request.Id);
        }

        // Map Entity sang DTO
        return _mapper.Map<OrderDto>(order);
    }
}
