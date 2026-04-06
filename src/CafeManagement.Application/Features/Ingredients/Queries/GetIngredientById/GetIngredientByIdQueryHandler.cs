using AutoMapper;
using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Application.DTOs.Ingredients;
using CafeManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Features.Ingredients.Queries.GetIngredientById;

/// <summary>
/// Handler xử lý query lấy chi tiết nguyên liệu
/// </summary>
public class GetIngredientByIdQueryHandler : IRequestHandler<GetIngredientByIdQuery, IngredientDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetIngredientByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IngredientDto> Handle(GetIngredientByIdQuery request, CancellationToken cancellationToken)
    {
        var ingredient = await _context.Ingredients
            .FirstOrDefaultAsync(i => i.Id == request.Id && !i.IsDeleted, cancellationToken);

        if (ingredient == null)
        {
            throw new NotFoundException("Ingredient", request.Id);
        }

        return _mapper.Map<IngredientDto>(ingredient);
    }
}
