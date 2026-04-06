using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Domain.Entities;
using CafeManagement.Domain.Enums;
using CafeManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTime _dateTime;
    private readonly ICurrentUserService _currentUser;

    public CreateOrderCommandHandler(
        IApplicationDbContext context,
        IDateTime dateTime,
        ICurrentUserService currentUser)
    {
        _context = context;
        _dateTime = dateTime;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate products exist và lấy thông tin
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await _context.Products
            .Include(p => p.ProductIngredients)
            .ThenInclude(pi => pi.Ingredient)
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        if (products.Count != productIds.Distinct().Count())
        {
            throw new NotFoundException("Product", "Một hoặc nhiều sản phẩm không tồn tại");
        }

        // 2. Kiểm tra tồn kho nguyên liệu TRƯỚC KHI tạo đơn
        await ValidateInventoryAsync(request.Items, products, cancellationToken);

        // 3. Tạo Order
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = await GenerateOrderNumberAsync(cancellationToken),
            OrderDate = _dateTime.Now,
            Status = OrderStatus.Pending,
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            Notes = request.Notes,
            DiscountAmount = request.DiscountAmount ?? 0
        };

        // 4. Tạo OrderItems và tính tổng tiền
        decimal totalAmount = 0;
        foreach (var item in request.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price,
                TotalPrice = product.Price * item.Quantity,
                Notes = item.Notes
            };

            order.OrderItems.Add(orderItem);
            totalAmount += orderItem.TotalPrice;
        }

        order.TotalAmount = totalAmount;
        order.FinalAmount = totalAmount - order.DiscountAmount.GetValueOrDefault();

        // 5. Trừ nguyên liệu trong kho (CORE LOGIC)
        await DeductInventoryAsync(request.Items, products, cancellationToken);

        // 6. Lưu Order vào database
        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        return order.Id;
    }

    /// <summary>
    /// Validate đủ nguyên liệu để làm đơn hàng
    /// </summary>
    private async Task ValidateInventoryAsync(
        List<CreateOrderItemDto> orderItems,
        List<Product> products,
        CancellationToken cancellationToken)
    {
        // Tính tổng nguyên liệu cần cho toàn bộ đơn hàng
        var requiredIngredients = new Dictionary<Guid, decimal>();

        foreach (var orderItem in orderItems)
        {
            var product = products.First(p => p.Id == orderItem.ProductId);

            foreach (var productIngredient in product.ProductIngredients)
            {
                var totalRequired = productIngredient.QuantityRequired * orderItem.Quantity;

                if (requiredIngredients.ContainsKey(productIngredient.IngredientId))
                {
                    requiredIngredients[productIngredient.IngredientId] += totalRequired;
                }
                else
                {
                    requiredIngredients[productIngredient.IngredientId] = totalRequired;
                }
            }
        }

        // Kiểm tra tồn kho
        foreach (var (ingredientId, requiredQuantity) in requiredIngredients)
        {
            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Id == ingredientId, cancellationToken);

            if (ingredient == null)
            {
                throw new NotFoundException("Ingredient", ingredientId);
            }

            if (ingredient.QuantityInStock < requiredQuantity)
            {
                throw new InsufficientStockException(
                    ingredient.Name,
                    requiredQuantity,
                    ingredient.QuantityInStock);
            }
        }
    }

    /// <summary>
    /// Trừ nguyên liệu trong kho (Recipe Engine)
    /// </summary>
    private async Task DeductInventoryAsync(
        List<CreateOrderItemDto> orderItems,
        List<Product> products,
        CancellationToken cancellationToken)
    {
        var ingredientUpdates = new Dictionary<Guid, decimal>();

        // Tính tổng nguyên liệu cần trừ
        foreach (var orderItem in orderItems)
        {
            var product = products.First(p => p.Id == orderItem.ProductId);

            foreach (var productIngredient in product.ProductIngredients)
            {
                var totalToDeduct = productIngredient.QuantityRequired * orderItem.Quantity;

                if (ingredientUpdates.ContainsKey(productIngredient.IngredientId))
                {
                    ingredientUpdates[productIngredient.IngredientId] += totalToDeduct;
                }
                else
                {
                    ingredientUpdates[productIngredient.IngredientId] = totalToDeduct;
                }
            }
        }

        // Cập nhật tồn kho
        foreach (var (ingredientId, quantityToDeduct) in ingredientUpdates)
        {
            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Id == ingredientId, cancellationToken);

            if (ingredient != null)
            {
                ingredient.QuantityInStock -= quantityToDeduct;
            }
        }
    }

    /// <summary>
    /// Generate mã đơn hàng tự động (ORD-YYYYMMDD-XXXX)
    /// </summary>
    private async Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken)
    {
        var today = _dateTime.Now.Date;
        var prefix = $"ORD-{today:yyyyMMdd}";

        var lastOrder = await _context.Orders
            .Where(o => o.OrderNumber.StartsWith(prefix))
            .OrderByDescending(o => o.OrderNumber)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastOrder == null)
        {
            return $"{prefix}-0001";
        }

        var lastNumber = int.Parse(lastOrder.OrderNumber.Split('-').Last());
        return $"{prefix}-{(lastNumber + 1):D4}";
    }
}
