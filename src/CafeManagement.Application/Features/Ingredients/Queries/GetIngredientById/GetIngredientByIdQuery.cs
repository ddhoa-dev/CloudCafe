using CafeManagement.Application.DTOs.Ingredients;
using MediatR;

namespace CafeManagement.Application.Features.Ingredients.Queries.GetIngredientById;

/// <summary>
/// Query để lấy chi tiết 1 nguyên liệu
/// </summary>
public class GetIngredientByIdQuery : IRequest<IngredientDto>
{
    public Guid Id { get; set; }

    public GetIngredientByIdQuery(Guid id)
    {
        Id = id;
    }
}
