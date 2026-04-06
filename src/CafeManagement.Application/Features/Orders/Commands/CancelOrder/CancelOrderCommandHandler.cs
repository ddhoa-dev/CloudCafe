using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Domain.Enums;
using CafeManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Features.Orders.Commands.CancelOrder;

/// <summary>
/// Handler xử lý việc hủy đơn hàng
/// CORE LOGIC: Tự động hoàn trả nguyên liệu vào kho khi hủy đơn
/// </summary>
public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public CancelOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Xử lý hủy đơn hàng theo các bước:
    /// 1. Validate đơn hàng tồn tại
    /// 2. Kiểm tra trạng thái (chỉ cho phép hủy Pending/Preparing)
    /// 3. Hoàn trả nguyên liệu vào kho
    /// 4. Cập nhật trạng thái = Cancelled
    /// 5. Ghi lý do hủy vào Notes
    /// </summary>
    public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        // ===== BƯỚC 1: Lấy thông tin đơn hàng =====
        // Include tất cả thông tin cần thiết để tính toán nguyên liệu
        var order = await _context.Orders
            .Include(o => o.OrderItems)                      // Chi tiết đơn hàng
            .ThenInclude(oi => oi.Product)                   // Thông tin sản phẩm
            .ThenInclude(p => p.ProductIngredients)          // Công thức sản phẩm
            .ThenInclude(pi => pi.Ingredient)                // Thông tin nguyên liệu
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
        {
            throw new NotFoundException("Order", request.OrderId);
        }

        // ===== BƯỚC 2: Validate trạng thái đơn hàng =====
        // Chỉ cho phép hủy đơn ở trạng thái Pending (Chờ xử lý) hoặc Preparing (Đang pha chế)
        // Không cho phép hủy đơn đã Ready, Completed, hoặc đã Cancelled
        if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Preparing)
        {
            throw new DomainException($"Không thể hủy đơn hàng ở trạng thái {order.Status}");
        }

        // ===== BƯỚC 3: Hoàn trả nguyên liệu vào kho =====
        // Ngược lại với CreateOrder: CỘNG lại nguyên liệu đã trừ
        await RestoreInventoryAsync(order, cancellationToken);

        // ===== BƯỚC 4: Cập nhật trạng thái và ghi lý do =====
        order.Status = OrderStatus.Cancelled;
        if (!string.IsNullOrEmpty(request.CancellationReason))
        {
            order.Notes = $"{order.Notes}\n[Hủy đơn] {request.CancellationReason}";
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    /// <summary>
    /// Hoàn trả nguyên liệu vào kho khi hủy đơn
    /// Ngược lại với DeductInventory: CỘNG lại nguyên liệu đã trừ
    /// 
    /// Ví dụ:
    /// - Đơn đã tạo: 2 Cafe Sữa → Đã trừ 40g Cafe, 200ml Sữa
    /// - Hủy đơn → Hoàn trả: +40g Cafe, +200ml Sữa
    /// </summary>
    private async Task RestoreInventoryAsync(Domain.Entities.Order order, CancellationToken cancellationToken)
    {
        // Dictionary để lưu tổng nguyên liệu cần hoàn trả: <IngredientId, TotalQuantity>
        var ingredientRestores = new Dictionary<Guid, decimal>();

        // ===== BƯỚC 1: Tính tổng nguyên liệu cần hoàn trả =====
        foreach (var orderItem in order.OrderItems)
        {
            // Duyệt qua công thức của từng sản phẩm trong đơn
            foreach (var productIngredient in orderItem.Product.ProductIngredients)
            {
                // Tính số lượng cần hoàn trả = QuantityRequired × Quantity
                var totalToRestore = productIngredient.QuantityRequired * orderItem.Quantity;

                // Cộng dồn nếu nguyên liệu đã có trong dictionary
                if (ingredientRestores.ContainsKey(productIngredient.IngredientId))
                {
                    ingredientRestores[productIngredient.IngredientId] += totalToRestore;
                }
                else
                {
                    ingredientRestores[productIngredient.IngredientId] = totalToRestore;
                }
            }
        }

        // ===== BƯỚC 2: Cập nhật tồn kho (CỘNG lại) =====
        foreach (var (ingredientId, quantityToRestore) in ingredientRestores)
        {
            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Id == ingredientId, cancellationToken);

            if (ingredient != null)
            {
                // CỘNG lại nguyên liệu vào kho (ngược lại với DeductInventory)
                ingredient.QuantityInStock += quantityToRestore;
            }
        }
    }
}
}
