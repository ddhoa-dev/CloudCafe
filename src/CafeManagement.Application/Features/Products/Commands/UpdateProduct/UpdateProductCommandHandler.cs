using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Domain.Entities;
using CafeManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// Handler xử lý cập nhật sản phẩm
/// </summary>
public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Cập nhật sản phẩm theo các bước:
    /// 1. Tìm sản phẩm cần update
    /// 2. Cập nhật thông tin cơ bản
    /// 3. Xóa công thức cũ
    /// 4. Tạo công thức mới
    /// 5. Lưu thay đổi
    /// </summary>
    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // ===== BƯỚC 1: Tìm sản phẩm =====
        var product = await _context.Products
            .Include(p => p.ProductIngredients)  // Load công thức hiện tại
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
        {
            throw new NotFoundException("Product", request.Id);
        }

        // ===== BƯỚC 2: Cập nhật thông tin cơ bản =====
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Category = request.Category;
        product.IsAvailable = request.IsAvailable;
        product.ImageUrl = request.ImageUrl;

        // ===== BƯỚC 3: Xóa công thức cũ =====
        // Remove tất cả ProductIngredient hiện tại
        product.ProductIngredients.Clear();

        // ===== BƯỚC 4: Tạo công thức mới =====
        if (request.Ingredients.Any())
        {
            // Validate nguyên liệu tồn tại
            var ingredientIds = request.Ingredients.Select(i => i.IngredientId).ToList();
            var existingIngredients = await _context.Ingredients
                .Where(i => ingredientIds.Contains(i.Id))
                .Select(i => i.Id)
                .ToListAsync(cancellationToken);

            var missingIngredients = ingredientIds.Except(existingIngredients).ToList();
            if (missingIngredients.Any())
            {
                throw new NotFoundException("Ingredient",
                    $"Không tìm thấy nguyên liệu với ID: {string.Join(", ", missingIngredients)}");
            }

            // Thêm công thức mới
            foreach (var ingredientDto in request.Ingredients)
            {
                var productIngredient = new ProductIngredient
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    IngredientId = ingredientDto.IngredientId,
                    QuantityRequired = ingredientDto.QuantityRequired
                };

                product.ProductIngredients.Add(productIngredient);
            }
        }

        // ===== BƯỚC 5: Lưu thay đổi =====
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
