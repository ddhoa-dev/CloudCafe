using CafeManagement.Application.Common.Models;
using CafeManagement.Application.DTOs.Ingredients;
using MediatR;

namespace CafeManagement.Application.Features.Ingredients.Queries.GetIngredients;

/// <summary>
/// Query để lấy danh sách nguyên liệu với pagination và filter
/// </summary>
public class GetIngredientsQuery : IRequest<PaginatedList<IngredientDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool? IsLowStock { get; set; }  // Filter nguyên liệu sắp hết
    public string? SearchTerm { get; set; }  // Tìm kiếm theo tên
}
