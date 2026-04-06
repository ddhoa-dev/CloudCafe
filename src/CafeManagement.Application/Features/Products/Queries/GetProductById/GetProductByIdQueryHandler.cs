using AutoMapper;
using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Application.DTOs.Products;
using CafeManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Features.Products.Queries.GetProductById;

/// <summary>
/// Handler xử lý query lấy chi tiết sản phẩm
/// </summary>
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Lấy chi tiết sản phẩm bao gồm:
    /// - Thông tin sản phẩm
    /// - Công thức (ProductIngredients)
    /// - Thông tin nguyên liệu
    /// </summary>
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Include(p => p.ProductIngredients)
            .ThenInclude(pi => pi.Ingredient)
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

        if (product == null)
        {
            throw new NotFoundException("Product", request.Id);
        }

        return _mapper.Map<ProductDto>(product);
    }
}
