using CafeManagement.Application.DTOs.Orders;
using CafeManagement.Application.Features.Orders.Commands.CancelOrder;
using CafeManagement.Application.Features.Orders.Commands.CreateOrder;
using CafeManagement.Application.Features.Orders.Commands.UpdateOrderStatus;
using CafeManagement.Application.Features.Orders.Queries.GetOrderById;
using CafeManagement.Application.Features.Orders.Queries.GetOrders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeManagement.API.Controllers;

/// <summary>
/// Controller quản lý đơn hàng
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]  // Yêu cầu authentication cho tất cả endpoints
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lấy danh sách đơn hàng với pagination và filters
    /// </summary>
    /// <param name="query">Query parameters</param>
    /// <returns>Paginated list of orders</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders([FromQuery] GetOrdersQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Lấy chi tiết đơn hàng theo ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <returns>Order details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Tạo đơn hàng mới
    /// Tự động trừ nguyên liệu trong kho
    /// </summary>
    /// <param name="command">Order data</param>
    /// <returns>Created order ID</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var orderId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetOrderById), new { id = orderId }, orderId);
    }

    /// <summary>
    /// Cập nhật trạng thái đơn hàng
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="command">New status</param>
    /// <returns>No content</returns>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusCommand command)
    {
        command.OrderId = id;
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Hủy đơn hàng và hoàn trả nguyên liệu
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="command">Cancellation reason</param>
    /// <returns>No content</returns>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelOrder(Guid id, [FromBody] CancelOrderCommand command)
    {
        command.OrderId = id;
        await _mediator.Send(command);
        return NoContent();
    }
}
