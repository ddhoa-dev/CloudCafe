using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Domain.Entities;
using CafeManagement.Domain.Enums;
using CafeManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Features.Orders.Commands.CreateOrder;

/// <summary>
/// Handler xử lý việc tạo đơn hàng mới
/// CORE LOGIC: Tự động tính toán và trừ nguyên liệu trong kho (Recipe Engine)
/// </summary>
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

    /// <summary>
    /// Xử lý tạo đơn hàng theo các bước:
    /// 1. Validate sản phẩm tồn tại
    /// 2. Kiểm tra đủ nguyên liệu (throw exception nếu không đủ)
    /// 3. Tạo Order và OrderItems
    /// 4. Tính tổng tiền
    /// 5. Trừ nguyên liệu trong kho
    /// 6. Lưu vào database
    /// </summary>
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // ===== BƯỚC 1: Validate sản phẩm tồn tại =====
        // Lấy danh sách ProductId từ request
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        
        // Query database để lấy thông tin sản phẩm kèm công thức (ProductIngredients)
        // Include: Eager loading để load cả Ingredient data
        var products = await _context.Products
            .Include(p => p.ProductIngredients)      // Load công thức
            .ThenInclude(pi => pi.Ingredient)        // Load thông tin nguyên liệu
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        // Kiểm tra: Số lượng products tìm được phải bằng số lượng ProductId yêu cầu
        if (products.Count != productIds.Distinct().Count())
        {
            throw new NotFoundException("Product", "Một hoặc nhiều sản phẩm không tồn tại");
        }

        // ===== BƯỚC 2: Kiểm tra đủ nguyên liệu TRƯỚC KHI tạo đơn =====
        // Nếu không đủ nguyên liệu → throw InsufficientStockException
        await ValidateInventoryAsync(request.Items, products, cancellationToken);

        // ===== BƯỚC 3: Tạo Order entity =====
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = await GenerateOrderNumberAsync(cancellationToken), // ORD-20260406-0001
            OrderDate = _dateTime.Now,
            Status = OrderStatus.Pending,  // Trạng thái ban đầu
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            Notes = request.Notes,
            DiscountAmount = request.DiscountAmount ?? 0
        };

        // ===== BƯỚC 4: Tạo OrderItems và tính tổng tiền =====
        decimal totalAmount = 0;
        foreach (var item in request.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            
            // Tạo OrderItem cho mỗi sản phẩm trong đơn hàng
            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price,                    // Giá tại thời điểm đặt hàng
                TotalPrice = product.Price * item.Quantity,   // Tổng tiền = Giá × Số lượng
                Notes = item.Notes                            // Ghi chú đặc biệt (ít đường, nhiều đá...)
            };

            order.OrderItems.Add(orderItem);
            totalAmount += orderItem.TotalPrice;
        }

        // Tính tổng tiền cuối cùng sau khi trừ giảm giá
        order.TotalAmount = totalAmount;
        order.FinalAmount = totalAmount - order.DiscountAmount.GetValueOrDefault();

        // ===== BƯỚC 5: Trừ nguyên liệu trong kho (RECIPE ENGINE - CORE LOGIC) =====
        // Ví dụ: 2 ly Cafe Sữa → Trừ 40g Cafe, 200ml Sữa, 20g Đường
        await DeductInventoryAsync(request.Items, products, cancellationToken);

        // ===== BƯỚC 6: Lưu Order vào database =====
        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        return order.Id;
    }

    /// <summary>
    /// Validate đủ nguyên liệu để làm đơn hàng
    /// Ví dụ: 
    /// - Đơn hàng: 2 Cafe Sữa + 1 Cafe Đen
    /// - Cafe Sữa cần: 20g Cafe, 100ml Sữa
    /// - Cafe Đen cần: 25g Cafe
    /// - Tổng cần: 65g Cafe (20×2 + 25×1), 200ml Sữa (100×2)
    /// </summary>
    private async Task ValidateInventoryAsync(
        List<CreateOrderItemDto> orderItems,
        List<Product> products,
        CancellationToken cancellationToken)
    {
        // Dictionary để lưu tổng nguyên liệu cần: <IngredientId, TotalQuantity>
        var requiredIngredients = new Dictionary<Guid, decimal>();

        // Duyệt qua từng item trong đơn hàng
        foreach (var orderItem in orderItems)
        {
            var product = products.First(p => p.Id == orderItem.ProductId);

            // Duyệt qua từng nguyên liệu trong công thức sản phẩm
            foreach (var productIngredient in product.ProductIngredients)
            {
                // Tính tổng nguyên liệu cần = QuantityRequired × Số lượng sản phẩm
                // Ví dụ: 2 ly Cafe Sữa × 20g Cafe = 40g Cafe
                var totalRequired = productIngredient.QuantityRequired * orderItem.Quantity;

                // Cộng dồn nếu nguyên liệu đã tồn tại trong dictionary
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

        // Kiểm tra từng nguyên liệu có đủ trong kho không
        foreach (var (ingredientId, requiredQuantity) in requiredIngredients)
        {
            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Id == ingredientId, cancellationToken);

            if (ingredient == null)
            {
                throw new NotFoundException("Ingredient", ingredientId);
            }

            // So sánh: Tồn kho < Cần dùng → Throw exception
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
    /// Trừ nguyên liệu trong kho (Recipe Engine - CORE LOGIC)
    /// Đây là phần quan trọng nhất: Tự động tính toán và trừ nguyên liệu
    /// 
    /// Flow:
    /// 1. Tính tổng nguyên liệu cần trừ cho toàn bộ đơn hàng
    /// 2. Cập nhật Ingredient.QuantityInStock -= totalToDeduct
    /// 
    /// Ví dụ:
    /// - Đơn hàng: 2 Cafe Sữa
    /// - Công thức Cafe Sữa: 20g Cafe + 100ml Sữa + 10g Đường
    /// - Kết quả: Trừ 40g Cafe, 200ml Sữa, 20g Đường
    /// </summary>
    private async Task DeductInventoryAsync(
        List<CreateOrderItemDto> orderItems,
        List<Product> products,
        CancellationToken cancellationToken)
    {
        // Dictionary để lưu tổng nguyên liệu cần trừ: <IngredientId, TotalQuantity>
        var ingredientUpdates = new Dictionary<Guid, decimal>();

        // ===== BƯỚC 1: Tính tổng nguyên liệu cần trừ =====
        foreach (var orderItem in orderItems)
        {
            var product = products.First(p => p.Id == orderItem.ProductId);

            // Duyệt qua công thức của sản phẩm
            foreach (var productIngredient in product.ProductIngredients)
            {
                // Tính số lượng cần trừ = QuantityRequired × Quantity
                var totalToDeduct = productIngredient.QuantityRequired * orderItem.Quantity;

                // Cộng dồn nếu nguyên liệu đã có trong dictionary
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

        // ===== BƯỚC 2: Cập nhật tồn kho trong database =====
        foreach (var (ingredientId, quantityToDeduct) in ingredientUpdates)
        {
            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Id == ingredientId, cancellationToken);

            if (ingredient != null)
            {
                // TRỪ nguyên liệu trong kho
                ingredient.QuantityInStock -= quantityToDeduct;
                
                // EF Core sẽ track thay đổi này và update khi SaveChanges()
            }
        }
    }

    /// <summary>
    /// Generate mã đơn hàng tự động theo format: ORD-YYYYMMDD-XXXX
    /// 
    /// Ví dụ:
    /// - Ngày 06/04/2026, đơn đầu tiên: ORD-20260406-0001
    /// - Đơn thứ 2 cùng ngày: ORD-20260406-0002
    /// - Đơn ngày mới: ORD-20260407-0001 (reset về 0001)
    /// </summary>
    private async Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken)
    {
        var today = _dateTime.Now.Date;
        var prefix = $"ORD-{today:yyyyMMdd}";  // ORD-20260406

        // Tìm đơn hàng cuối cùng trong ngày
        var lastOrder = await _context.Orders
            .Where(o => o.OrderNumber.StartsWith(prefix))
            .OrderByDescending(o => o.OrderNumber)
            .FirstOrDefaultAsync(cancellationToken);

        // Nếu chưa có đơn nào trong ngày → Bắt đầu từ 0001
        if (lastOrder == null)
        {
            return $"{prefix}-0001";
        }

        // Parse số thứ tự từ OrderNumber cuối cùng
        // Ví dụ: "ORD-20260406-0005" → Split → ["ORD", "20260406", "0005"] → Last() = "0005"
        var lastNumber = int.Parse(lastOrder.OrderNumber.Split('-').Last());
        
        // Tăng lên 1 và format thành 4 chữ số (D4)
        return $"{prefix}-{(lastNumber + 1):D4}";
    }
}
