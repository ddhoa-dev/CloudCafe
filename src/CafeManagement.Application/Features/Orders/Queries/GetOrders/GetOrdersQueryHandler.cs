using AutoMapper;
using AutoMapper.QueryableExtensions;
using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Application.Common.Models;
using CafeManagement.Application.DTOs.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Features.Orders.Queries.GetOrders;

/// <summary>
/// Handler xử lý query lấy danh sách đơn hàng
/// Hỗ trợ: Pagination, Filter theo Status/Date/Phone
/// </summary>
public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PaginatedList<OrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetOrdersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Xử lý query với các bước:
    /// 1. Build query với filters
    /// 2. Apply pagination
    /// 3. Map sang DTO
    /// 4. Return PaginatedList
    /// </summary>
    public async Task<PaginatedList<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        // ===== BƯỚC 1: Build base query =====
        // Include OrderItems và Product để map sang DTO
        var query = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .AsQueryable();

        // ===== BƯỚC 2: Apply filters (nếu có) =====

        // Filter theo trạng thái đơn hàng
        if (request.Status.HasValue)
        {
            query = query.Where(o => o.Status == request.Status.Value);
        }

        // Filter theo ngày bắt đầu
        if (request.FromDate.HasValue)
        {
            query = query.Where(o => o.OrderDate >= request.FromDate.Value);
        }

        // Filter theo ngày kết thúc
        if (request.ToDate.HasValue)
        {
            query = query.Where(o => o.OrderDate <= request.ToDate.Value);
        }

        // Filter theo số điện thoại khách hàng
        if (!string.IsNullOrEmpty(request.CustomerPhone))
        {
            query = query.Where(o => o.CustomerPhone == request.CustomerPhone);
        }

        // ===== BƯỚC 3: Sắp xếp =====
        // Đơn hàng mới nhất lên đầu
        query = query.OrderByDescending(o => o.OrderDate);

        // ===== BƯỚC 4: Đếm tổng số records (trước khi pagination) =====
        var totalCount = await query.CountAsync(cancellationToken);

        // ===== BƯỚC 5: Apply pagination =====
        // Skip: Bỏ qua (PageNumber - 1) × PageSize records
        // Take: Lấy PageSize records
        // Ví dụ: Page 2, Size 10 → Skip(10), Take(10) → Records 11-20
        var orders = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)  // AutoMapper projection
            .ToListAsync(cancellationToken);

        // ===== BƯỚC 6: Return PaginatedList =====
        return new PaginatedList<OrderDto>(orders, totalCount, request.PageNumber, request.PageSize);
    }
}
