using AutoMapper;
using AutoMapper.QueryableExtensions;
using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Application.Common.Models;
using CafeManagement.Application.DTOs.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Features.Products.Queries.GetProducts;

/// <summary>
/// Handler xử lý query lấy danh sách sản phẩm
/// Hỗ trợ: Pagination, Filter theo Category/IsAvailable, Search theo tên
/// </summary>
public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedList<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        // ===== BƯỚC 1: Build base query =====
        var query = _context.Products
            .Include(p => p.ProductIngredients)
            .ThenInclude(pi => pi.Ingredient)
            .Where(p => !p.IsDeleted)  // Chỉ lấy sản phẩm chưa bị xóa
            .AsQueryable();

        // ===== BƯỚC 2: Apply filters =====

        // Filter theo category
        if (request.Category.HasValue)
        {
            query = query.Where(p => p.Category == request.Category.Value);
        }

        // Filter theo availability
        if (request.IsAvailable.HasValue)
        {
            query = query.Where(p => p.IsAvailable == request.IsAvailable.Value);
        }

        // Search theo tên sản phẩm
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(p => p.Name.Contains(request.SearchTerm));
        }

        // ===== BƯỚC 3: Sắp xếp =====
        query = query.OrderBy(p => p.Name);

        // ===== BƯỚC 4: Đếm tổng số records =====
        var totalCount = await query.CountAsync(cancellationToken);

        // ===== BƯỚC 5: Apply pagination =====
        var products = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PaginatedList<ProductDto>(products, totalCount, request.PageNumber, request.PageSize);
    }
}
