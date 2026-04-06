# Cloud-Native Cafe Management & Inventory System

Hệ thống quản lý quán Cafe và tồn kho hướng Cloud được xây dựng với .NET 8/9 và Clean Architecture.

## Tech Stack

- **Framework**: .NET 8/9 (ASP.NET Core Web API)
- **Database**: PostgreSQL với Entity Framework Core
- **Architecture**: Clean Architecture (Onion Architecture)
- **Security**: JWT Authentication & Role-based Authorization
- **Tools**: FluentValidation, AutoMapper, Serilog, MediatR
- **DevOps**: Docker, Docker Compose, GitHub Actions

## Cấu trúc dự án

```
CafeManagement/
├── src/
│   ├── CafeManagement.Domain/          # Entities, Interfaces, Domain Logic
│   ├── CafeManagement.Application/     # DTOs, CQRS, Business Logic
│   ├── CafeManagement.Infrastructure/  # DbContext, Repositories, External Services
│   └── CafeManagement.API/             # Controllers, Middlewares, DI Configuration
├── tests/
│   ├── CafeManagement.UnitTests/
│   └── CafeManagement.IntegrationTests/
├── docker-compose.yml
├── .github/
│   └── workflows/
│       └── ci-cd.yml
└── README.md
```

## Tính năng cốt lõi

1. **Inventory & Recipe Engine**: Tự động tính toán và trừ nguyên liệu khi tạo đơn hàng
2. **Auditing**: Tự động lưu thông tin CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy
3. **CQRS Pattern**: Sử dụng MediatR để tách biệt Command và Query
4. **Validation**: FluentValidation cho tất cả Input
5. **Logging**: Serilog với JSON format

## Hướng dẫn chạy dự án

### Yêu cầu

- .NET 8 SDK trở lên
- Docker & Docker Compose
- PostgreSQL (hoặc dùng Docker)

### Biến môi trường

Tạo file `appsettings.Development.json` trong thư mục API:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=CafeManagementDb;Username=postgres;Password=your_password"
  },
  "JwtSettings": {
    "Secret": "your-super-secret-key-min-32-characters-long",
    "Issuer": "CafeManagementAPI",
    "Audience": "CafeManagementClient",
    "ExpiryMinutes": 60
  }
}
```

### Chạy với Docker Compose

```bash
docker-compose up -d
```

### Chạy local

```bash
cd src/CafeManagement.API
dotnet restore
dotnet ef database update
dotnet run
```

Truy cập Swagger UI: `https://localhost:5001/swagger`
