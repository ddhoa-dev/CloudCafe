using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Domain.Entities;
using CafeManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Handler xử lý tạo sản phẩm mới kèm công thức
/// </summary>
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Xử lý tạo sản phẩm theo các bước:
    /// 1. Validate nguyên liệu tồn tại
    /// 2. Tạo Product entity
    /// 3. Tạo ProductIngredient (công thức) cho từng nguyên liệu
    /// 4. Lưu vào database
    /// </summary>
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // ===== BƯỚC 1: Validate nguyên liệu tồn tại =====
        if (request.Ingredients.Any())
        {
            var ingredientIds = request.Ingredients.Select(i => i.IngredientId).ToList();
            var existingIngredients = await _context.Ingredients
                .Where(i => ingredientIds.Contains(i.Id))
                .Select(i => i.Id)
                .ToListAsync(cancellationToken);

            // Kiểm tra tất cả IngredientId có tồn tại không
            var missingIngredients = ingredientIds.Except(existingIngredients).ToList();
            if (missingIngredients.Any())
            {
                throw new NotFoundException("Ingredient", 
                    $"Không tìm thấy nguyên liệu với ID: {string.Join(", ", missingIngredients)}");
            }
        }

        // ===== BƯỚC 2: Tạo Product entity =====
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            ImageUrl = request.ImageUrl,
            IsAvailable = true  // Mặc định sản phẩm available
        };

        // ===== BƯỚC 3: Tạo công thức (ProductIngredient) =====
        // Ví dụ: Cafe Sữa = 20g Cafe + 100ml Sữa + 10g Đường
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

        // ===== BƯỚC 4: Lưu vào database =====
        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
