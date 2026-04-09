using AutoMapper;
using AutoMapper.QueryableExtensions;
using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Application.Common.Models;
using CafeManagement.Application.DTOs.Ingredients;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Features.Ingredients.Queries.GetIngredients;

/// <summary>
/// Handler xử lý query lấy danh sách nguyên liệu
/// Hỗ trợ: Pagination, Filter nguyên liệu sắp hết, Search theo tên
/// </summary>
public class GetIngredientsQueryHandler : IRequestHandler<GetIngredientsQuery, PaginatedList<IngredientDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetIngredientsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<IngredientDto>> Handle(GetIngredientsQuery request, CancellationToken cancellationToken)
    {
        // ===== BƯỚC 1: Build base query =====
        var query = _context.Ingredients
            .Where(i => !i.IsDeleted)
            .AsQueryable();

        // ===== BƯỚC 2: Apply filters =====

        // Filter nguyên liệu sắp hết (QuantityInStock <= MinimumStockLevel)
        if (request.IsLowStock.HasValue && request.IsLowStock.Value)
        {
            query = query.Where(i => i.QuantityInStock <= i.MinimumStockLevel);
        }

        // Search theo tên nguyên liệu
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(i => i.Name.Contains(request.SearchTerm));
        }

        // ===== BƯỚC 3: Sắp xếp =====
        query = query.OrderBy(i => i.Name);

        // ===== BƯỚC 4: Đếm tổng số records =====
        var totalCount = await query.CountAsync(cancellationToken);

        // ===== BƯỚC 5: Apply pagination =====
        var ingredients = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<IngredientDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PaginatedList<IngredientDto>(ingredients, totalCount, request.PageNumber, request.PageSize);
    }
}
