using CafeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedDataAsync(ApplicationDbContext context)
    {
        // Kiểm tra xem database đã có dữ liệu chưa
        if (await context.Products.AnyAsync() || await context.Ingredients.AnyAsync())
        {
            return; // Đã có dữ liệu thì không chạy script này nữa
        }

        // ===== 1. TẠO DỮ LIỆU NGUYÊN LIỆU (INGREDIENTS) =====
        var cafe = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = "Cà phê hạt (Robusta)",
            Description = "Hạt Robusta nguyên chất Tây Nguyên",
            Unit = "g",
            QuantityInStock = 5000,     // Trong kho có 5kg (5000 gram)
            MinimumStockLevel = 1000,   // Báo động nếu dưới 1kg
            UnitPrice = 200,            // Giá cốt: 200đ / 1 gram
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System Seeder"
        };

        var suaDac = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = "Sữa đặc",
            Description = "Sữa đặc Ngôi Sao Phương Nam",
            Unit = "ml",
            QuantityInStock = 10000,    // 10 Lít
            MinimumStockLevel = 2000,
            UnitPrice = 60,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System Seeder"
        };

        var kemBeo = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = "Kem béo Rich's",
            Description = "Dùng pha trà sữa, bạc xỉu",
            Unit = "ml",
            QuantityInStock = 3000,
            MinimumStockLevel = 500,
            UnitPrice = 100,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System Seeder"
        };

        await context.Ingredients.AddRangeAsync(cafe, suaDac, kemBeo);

        // ===== 2. TẠO SẢN PHẨM (PRODUCTS) =====
        var cfDen = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Cà Phê Đen Đá",
            Description = "Đậm đà hương vị nguyên bản",
            Price = 25000,
            ImageUrl = "cf-den",
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System Seeder"
        };

        var cfSua = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Cà Phê Sữa Đá",
            Description = "Ngọt ngào, thơm béo",
            Price = 30000,
            ImageUrl = "cf-sua",
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System Seeder"
        };

        var bacXiu = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Bạc Xỉu",
            Description = "Nhiều sữa, ít cà phê",
            Price = 35000,
            ImageUrl = "bac-xiu",
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System Seeder"
        };

        await context.Products.AddRangeAsync(cfDen, cfSua, bacXiu);

        // ===== 3. TẠO CÔNG THỨC (RECIPES / PRODUCT INGREDIENTS) =====

        // Cà phê đen (25g cafe)
        var rpCfDen1 = new ProductIngredient
        {
            Id = Guid.NewGuid(),
            ProductId = cfDen.Id,
            IngredientId = cafe.Id,
            QuantityRequired = 25
        };

        // Cà phê sữa (20g cafe + 30ml sữa đặc)
        var rpCfSua1 = new ProductIngredient
        {
            Id = Guid.NewGuid(),
            ProductId = cfSua.Id,
            IngredientId = cafe.Id,
            QuantityRequired = 20
        };
        var rpCfSua2 = new ProductIngredient
        {
            Id = Guid.NewGuid(),
            ProductId = cfSua.Id,
            IngredientId = suaDac.Id,
            QuantityRequired = 30
        };

        // Bạc xỉu (10g cafe + 40ml sữa đặc + 20ml kem béo)
        var rpBacXiu1 = new ProductIngredient
        {
            Id = Guid.NewGuid(),
            ProductId = bacXiu.Id,
            IngredientId = cafe.Id,
            QuantityRequired = 10
        };
        var rpBacXiu2 = new ProductIngredient
        {
            Id = Guid.NewGuid(),
            ProductId = bacXiu.Id,
            IngredientId = suaDac.Id,
            QuantityRequired = 40
        };
        var rpBacXiu3 = new ProductIngredient
        {
            Id = Guid.NewGuid(),
            ProductId = bacXiu.Id,
            IngredientId = kemBeo.Id,
            QuantityRequired = 20
        };

        await context.ProductIngredients.AddRangeAsync(
            rpCfDen1, rpCfSua1, rpCfSua2,
            rpBacXiu1, rpBacXiu2, rpBacXiu3
        );

        // LƯU TOÀN BỘ VÀO DB
        await context.SaveChangesAsync();
    }
}
