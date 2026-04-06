using CafeManagement.Application.Common.Models;
using CafeManagement.Application.DTOs.Products;
using CafeManagement.Domain.Enums;
using MediatR;

namespace CafeManagement.Application.Features.Products.Queries.GetProducts;

/// <summary>
/// Query để lấy danh sách sản phẩm với pagination và filter
/// </summary>
public class GetProductsQuery : IRequest<PaginatedList<ProductDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public ProductCategory? Category { get; set; }
    public bool? IsAvailable { get; set; }
    public string? SearchTerm { get; set; }  // Tìm kiếm theo tên
}
