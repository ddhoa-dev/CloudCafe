# Hướng dẫn Setup và Chạy dự án

## Yêu cầu hệ thống

- .NET 8 SDK
- PostgreSQL 16 (hoặc dùng Docker)
- Docker & Docker Compose (optional)

## Cách 1: Chạy với Docker Compose (Khuyến nghị)

### Bước 1: Clone repository
```bash
git clone <repository-url>
cd CafeManagement
```

### Bước 2: Chạy Docker Compose
```bash
docker-compose up -d
```

Lệnh này sẽ khởi động:
- PostgreSQL database (port 5432)
- pgAdmin (port 5050)
- API service (port 5000)

### Bước 3: Truy cập ứng dụng

- **API Swagger**: http://localhost:5000
- **pgAdmin**: http://localhost:5050
  - Email: admin@cafe.com
  - Password: admin

### Bước 4: Chạy Migrations

```bash
# Vào container API
docker exec -it cafe-api bash

# Chạy migrations
dotnet ef database update --project /src/src/CafeManagement.Infrastructure --startup-project /src/src/CafeManagement.API
```

## Cách 2: Chạy Local (Development)

### Bước 1: Cài đặt PostgreSQL

Tải và cài đặt PostgreSQL từ: https://www.postgresql.org/download/

Tạo database:
```sql
CREATE DATABASE CafeManagementDb;
```

### Bước 2: Cấu hình Connection String

Sửa file `src/CafeManagement.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=CafeManagementDb;Username=postgres;Password=your_password"
  }
}
```

### Bước 3: Restore Dependencies

```bash
dotnet restore
```

### Bước 4: Chạy Migrations

```bash
cd src/CafeManagement.API
dotnet ef database update --project ../CafeManagement.Infrastructure
```

### Bước 5: Chạy API

```bash
dotnet run
```

Truy cập: https://localhost:5001/swagger

## Biến môi trường

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=CafeManagementDb;Username=postgres;Password=postgres"
  },
  "JwtSettings": {
    "Secret": "your-super-secret-key-must-be-at-least-32-characters-long",
    "Issuer": "CafeManagementAPI",
    "Audience": "CafeManagementClient",
    "ExpiryMinutes": 60
  }
}
```

## Các lệnh hữu ích

### Entity Framework Migrations

```bash
# Tạo migration mới
dotnet ef migrations add InitialCreate --project src/CafeManagement.Infrastructure --startup-project src/CafeManagement.API

# Apply migrations
dotnet ef database update --project src/CafeManagement.Infrastructure --startup-project src/CafeManagement.API

# Remove migration cuối cùng
dotnet ef migrations remove --project src/CafeManagement.Infrastructure --startup-project src/CafeManagement.API

# Xem SQL script
dotnet ef migrations script --project src/CafeManagement.Infrastructure --startup-project src/CafeManagement.API
```

### Docker Commands

```bash
# Build và start services
docker-compose up -d

# Stop services
docker-compose down

# View logs
docker-compose logs -f api

# Rebuild API image
docker-compose up -d --build api

# Remove all containers and volumes
docker-compose down -v
```

## Testing API

### 1. Tạo User (Register)
```bash
POST /api/auth/register
{
  "username": "admin",
  "email": "admin@cafe.com",
  "password": "Admin@123",
  "fullName": "Admin User",
  "role": 1
}
```

### 2. Login
```bash
POST /api/auth/login
{
  "username": "admin",
  "password": "Admin@123"
}
```

### 3. Tạo Ingredient
```bash
POST /api/ingredients
Authorization: Bearer <token>
{
  "name": "Cafe",
  "unit": "gram",
  "quantityInStock": 1000,
  "minimumStockLevel": 100,
  "unitPrice": 0.5
}
```

### 4. Tạo Product với Recipe
```bash
POST /api/products
Authorization: Bearer <token>
{
  "name": "Cafe Sữa",
  "price": 25000,
  "category": 1,
  "ingredients": [
    {
      "ingredientId": "<ingredient-id>",
      "quantityRequired": 20
    }
  ]
}
```

### 5. Tạo Order
```bash
POST /api/orders
Authorization: Bearer <token>
{
  "customerName": "Nguyễn Văn A",
  "items": [
    {
      "productId": "<product-id>",
      "quantity": 2
    }
  ]
}
```

## Troubleshooting

### Lỗi: Cannot connect to PostgreSQL
- Kiểm tra PostgreSQL đã chạy: `docker ps` hoặc `pg_isready`
- Kiểm tra connection string trong appsettings.json

### Lỗi: Migration failed
- Xóa database và tạo lại
- Xóa folder Migrations và tạo migration mới

### Lỗi: Port already in use
- Thay đổi port trong docker-compose.yml hoặc appsettings.json
- Kill process đang dùng port: `lsof -ti:5000 | xargs kill -9`

## Logs

Logs được lưu tại: `src/CafeManagement.API/logs/`

Format: `log-YYYYMMDD.txt`
