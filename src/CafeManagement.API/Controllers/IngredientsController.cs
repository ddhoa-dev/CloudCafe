using CafeManagement.Application.Features.Ingredients.Commands.CreateIngredient;
using CafeManagement.Application.Features.Ingredients.Commands.UpdateIngredient;
using CafeManagement.Application.Features.Ingredients.Queries.GetIngredientById;
using CafeManagement.Application.Features.Ingredients.Queries.GetIngredients;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeManagement.API.Controllers;

/// <summary>
/// Controller quản lý nguyên liệu
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IngredientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public IngredientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lấy danh sách nguyên liệu với pagination và filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIngredients([FromQuery] GetIngredientsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Lấy chi tiết nguyên liệu
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIngredientById(Guid id)
    {
        var result = await _mediator.Send(new GetIngredientByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Tạo nguyên liệu mới
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateIngredient([FromBody] CreateIngredientCommand command)
    {
        var ingredientId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetIngredientById), new { id = ingredientId }, ingredientId);
    }

    /// <summary>
    /// Cập nhật nguyên liệu (bao gồm tồn kho)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateIngredient(Guid id, [FromBody] UpdateIngredientCommand command)
    {
        command.Id = id;
        await _mediator.Send(command);
        return NoContent();
    }
}
