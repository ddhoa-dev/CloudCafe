using CafeManagement.Application.Features.Products.Commands.CreateProduct;
using CafeManagement.Application.Features.Products.Commands.DeleteProduct;
using CafeManagement.Application.Features.Products.Commands.UpdateProduct;
using CafeManagement.Application.Features.Products.Queries.GetProductById;
using CafeManagement.Application.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeManagement.API.Controllers;

/// <summary>
/// Controller quản lý sản phẩm
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lấy danh sách sản phẩm với pagination và filters
    /// </summary>
    [HttpGet]
    [AllowAnonymous]  // Cho phép truy cập không cần authentication
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts([FromQuery] GetProductsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Lấy chi tiết sản phẩm kèm công thức
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Tạo sản phẩm mới kèm công thức
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]  // Chỉ Admin và Manager mới được tạo
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        var productId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProductById), new { id = productId }, productId);
    }

    /// <summary>
    /// Cập nhật sản phẩm và công thức
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductCommand command)
    {
        command.Id = id;
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Xóa sản phẩm (Soft Delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]  // Chỉ Admin mới được xóa
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        await _mediator.Send(new DeleteProductCommand(id));
        return NoContent();
    }
}
