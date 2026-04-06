# Cloud-Native Cafe Management & Inventory System

Hệ thống quản lý quán Cafe và tồn kho hướng Cloud được xây dựng với .NET 8 và Clean Architecture.

[![CI/CD Pipeline](https://github.com/yourusername/CafeManagement/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/yourusername/CafeManagement/actions/workflows/ci-cd.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## 🚀 Tech Stack

- **Framework**: .NET 8 (ASP.NET Core Web API)
- **Database**: PostgreSQL 16 với Entity Framework Core
- **Architecture**: Clean Architecture (Onion Architecture)
- **Security**: JWT Authentication & Role-based Authorization
- **Patterns**: CQRS với MediatR, Repository Pattern, Unit of Work
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Logging**: Serilog (Console + File JSON format)
- **API Documentation**: Swagger/OpenAPI
- **DevOps**: Docker, Docker Compose, GitHub Actions CI/CD

## 📁 Cấu trúc dự án

```
CafeManagement/
├── src/
│   ├── CafeManagement.Domain/          # Entities, Interfaces, Domain Logic
│   ├── CafeManagement.Application/     # DTOs, CQRS, Business Logic
│   ├── CafeManagement.Infrastructure/  # DbContext, Repositories, Services
│   └── CafeManagement.API/             # Controllers, Middlewares, DI
├── tests/
│   ├── CafeManagement.UnitTests/
│   └── CafeManagement.IntegrationTests/
├── .github/
│   └── workflows/
│       ├── ci-cd.yml                   # CI/CD Pipeline
│       └── deploy.yml                  # Deployment Pipeline
├── docker-compose.yml
├── Dockerfile
├── SETUP.md                            # Hướng dẫn setup
├── DEPLOYMENT.md                       # Hướng dẫn deployment
└── README.md
```

## ✨ Tính năng cốt lõi

### 1. Recipe Engine (Công thức tự động)
- Mỗi sản phẩm có công thức gồm nhiều nguyên liệu với định lượng cụ thể
- Tự động tính toán và trừ nguyên liệu khi tạo đơn hàng
- Tự động hoàn trả nguyên liệu khi hủy đơn
- Cảnh báo khi nguyên liệu sắp hết

**Ví dụ:**
```
Cafe Sữa = 20g Cafe + 100ml Sữa + 10g Đường
→ Đặt 2 ly Cafe Sữa
→ Tự động trừ: 40g Cafe, 200ml Sữa, 20g Đường
```

### 2. Auditing tự động
- Tự động lưu CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy
- Sử dụng EF Core Interceptor
- Lấy thông tin user từ JWT token

### 3. CQRS Pattern
- Tách biệt Command (Write) và Query (Read)
- Sử dụng MediatR
- FluentValidation cho tất cả Commands

### 4. Role-based Authorization
- Admin: Toàn quyền
- Manager: Quản lý sản phẩm, nguyên liệu, đơn hàng
- Staff: Tạo đơn hàng, xem sản phẩm
- Cashier: Xem và thanh toán đơn hàng

### 5. Global Exception Handling
- Middleware xử lý tất cả exceptions
- Trả về JSON response thống nhất
- Log chi tiết với Serilog

## 🏃 Quick Start

### Với Docker Compose (Khuyến nghị)

```bash
# Clone repository
git clone <repository-url>
cd CafeManagement

# Start services
docker-compose up -d

# Chạy migrations
docker exec -it cafe-api dotnet ef database update --project /src/src/CafeManagement.Infrastructure --startup-project /src/src/CafeManagement.API
```

Truy cập:
- **API Swagger**: http://localhost:5000
- **pgAdmin**: http://localhost:5050 (admin@cafe.com / admin)

### Chạy Local

Xem chi tiết trong [SETUP.md](SETUP.md)

## 📚 API Endpoints

### Authentication
- `POST /api/auth/register` - Đăng ký user mới
- `POST /api/auth/login` - Đăng nhập
- `GET /api/auth/me` - Lấy thông tin user hiện tại

### Products
- `GET /api/products` - Danh sách sản phẩm (Public)
- `GET /api/products/{id}` - Chi tiết sản phẩm (Public)
- `POST /api/products` - Tạo sản phẩm (Admin/Manager)
- `PUT /api/products/{id}` - Cập nhật sản phẩm (Admin/Manager)
- `DELETE /api/products/{id}` - Xóa sản phẩm (Admin)

### Ingredients
- `GET /api/ingredients` - Danh sách nguyên liệu
- `GET /api/ingredients/{id}` - Chi tiết nguyên liệu
- `POST /api/ingredients` - Tạo nguyên liệu (Admin/Manager)
- `PUT /api/ingredients/{id}` - Cập nhật nguyên liệu (Admin/Manager)

### Orders
- `GET /api/orders` - Danh sách đơn hàng
- `GET /api/orders/{id}` - Chi tiết đơn hàng
- `POST /api/orders` - Tạo đơn hàng (tự động trừ kho)
- `PATCH /api/orders/{id}/status` - Cập nhật trạng thái
- `POST /api/orders/{id}/cancel` - Hủy đơn (hoàn trả kho)

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🚢 Deployment

Xem chi tiết trong [DEPLOYMENT.md](DEPLOYMENT.md)

### GitHub Actions CI/CD

Dự án đã được cấu hình với:
- ✅ Code quality check
- ✅ Build & Test
- ✅ Docker image build & push
- ✅ Security scan với Trivy
- ✅ Automated deployment

## 📊 Database Schema

```
Users
├── Id (PK)
├── Username (Unique)
├── Email (Unique)
├── PasswordHash
├── Role
└── Auditing fields

Products
├── Id (PK)
├── Name
├── Price
├── Category
└── ProductIngredients (FK)

Ingredients
├── Id (PK)
├── Name
├── QuantityInStock
├── MinimumStockLevel
└── UnitPrice

ProductIngredients (Recipe)
├── Id (PK)
├── ProductId (FK)
├── IngredientId (FK)
└── QuantityRequired

Orders
├── Id (PK)
├── OrderNumber (Unique)
├── Status
├── TotalAmount
└── OrderItems (FK)

OrderItems
├── Id (PK)
├── OrderId (FK)
├── ProductId (FK)
├── Quantity
└── TotalPrice
```

## 🔒 Security

- JWT Bearer Authentication
- Password hashing (SHA256 - nên dùng BCrypt trong production)
- Role-based Authorization
- HTTPS enforcement
- CORS configuration
- SQL Injection protection (EF Core parameterized queries)

## 📝 Logging

Logs được lưu tại `logs/log-YYYYMMDD.txt` với format JSON:

```json
{
  "Timestamp": "2026-04-06 10:30:45.123",
  "Level": "Information",
  "Message": "Order created successfully",
  "Properties": {
    "OrderId": "guid",
    "UserId": "guid"
  }
}
```

## 🤝 Contributing

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request

## 📄 License

This project is licensed under the MIT License.

## 👨‍💻 Author

Your Name - [your.email@example.com](mailto:your.email@example.com)

## 🙏 Acknowledgments

- Clean Architecture by Jason Taylor
- CQRS Pattern
- Domain-Driven Design
- .NET Community
