using AutoMapper;
using AutoMapper.QueryableExtensions;
using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Application.Common.Models;
using CafeManagement.Application.DTOs.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Features.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PaginatedList<OrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetOrdersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .AsQueryable();

        // Apply filters
        if (request.Status.HasValue)
        {
            query = query.Where(o => o.Status == request.Status.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(o => o.OrderDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(o => o.OrderDate <= request.ToDate.Value);
        }

        if (!string.IsNullOrEmpty(request.CustomerPhone))
        {
            query = query.Where(o => o.CustomerPhone == request.CustomerPhone);
        }

        // Order by date descending
        query = query.OrderByDescending(o => o.OrderDate);

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var orders = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PaginatedList<OrderDto>(orders, totalCount, request.PageNumber, request.PageSize);
    }
}
