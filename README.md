# Cloud-Native Cafe Management & Inventory System

Cloud-oriented Cafe Management and Inventory System built with .NET 8 and Clean Architecture.

[![CI/CD Pipeline](https://github.com/yourusername/CafeManagement/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/yourusername/CafeManagement/actions/workflows/ci-cd.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**Live Demo API (Swagger):** [https://cloudcafe.onrender.com/](https://cloudcafe.onrender.com/) *(Hosted on Render Cloud with PostgreSQL)*

## Tech Stack

- **Framework**: .NET 8 (ASP.NET Core Web API)
- **Database**: PostgreSQL 16 with Entity Framework Core
- **Architecture**: Clean Architecture (Onion Architecture)
- **Security**: JWT Authentication & Role-based Authorization
- **Patterns**: CQRS with MediatR, Repository Pattern, Unit of Work
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Logging**: Serilog (Console + File JSON format)
- **API Documentation**: Swagger/OpenAPI
- **DevOps**: Docker, Docker Compose, GitHub Actions CI/CD

## Project Structure

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
├── SETUP.md                            # Setup guide
├── DEPLOYMENT.md                       # Deployment guide
└── README.md
```

## Core Features

### 1. Recipe Engine (Automated Recipes)
- Each product has a recipe containing multiple ingredients with specific quantities.
- Automatically calculates and deducts ingredients when an order is created.
- Automatically restores ingredients when an order is canceled.
- Warning when an ingredient is low in stock.

**Example:**
```
Cafe Sữa = 20g Cafe + 100ml Milk + 10g Sugar
→ Order 2 cups of Cafe Sữa
→ Automatically deduct: 40g Cafe, 200ml Milk, 20g Sugar
```

### 2. Automated Auditing
- Automatically saves CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy
- Uses EF Core Interceptor
- Retrieves user information from JWT token

### 3. CQRS Pattern
- Separates Command (Write) and Query (Read)
- Uses MediatR
- FluentValidation for all Commands

### 4. Role-based Authorization
- Admin: Full access
- Manager: Manage products, ingredients, orders
- Staff: Create orders, view products
- Cashier: View and pay orders

### 5. Global Exception Handling
- Middleware handles all exceptions
- Returns a unified JSON response
- Detailed logging with Serilog

## Quick Start

### With Docker Compose (Recommended)

```bash
# Clone repository
git clone <repository-url>
cd CafeManagement

# Start services
docker-compose up -d

# Run migrations
docker exec -it cafe-api dotnet ef database update --project /src/src/CafeManagement.Infrastructure --startup-project /src/src/CafeManagement.API
```

Access:
- **API Swagger**: http://localhost:5000
- **pgAdmin**: http://localhost:5050 (admin@cafe.com / admin)

### Run Locally

See details in [SETUP.md](SETUP.md)

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login
- `GET /api/auth/me` - Get current user info

### Products
- `GET /api/products` - Product list (Public)
- `GET /api/products/{id}` - Product details (Public)
- `POST /api/products` - Create product (Admin/Manager)
- `PUT /api/products/{id}` - Update product (Admin/Manager)
- `DELETE /api/products/{id}` - Delete product (Admin)

### Ingredients
- `GET /api/ingredients` - Ingredient list
- `GET /api/ingredients/{id}` - Ingredient details
- `POST /api/ingredients` - Create ingredient (Admin/Manager)
- `PUT /api/ingredients/{id}` - Update ingredient (Admin/Manager)

### Orders
- `GET /api/orders` - Order list
- `GET /api/orders/{id}` - Order details
- `POST /api/orders` - Create order (auto deducts stock)
- `PATCH /api/orders/{id}/status` - Update status
- `POST /api/orders/{id}/cancel` - Cancel order (restores stock)

## Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Deployment

See details in [DEPLOYMENT.md](DEPLOYMENT.md)

### GitHub Actions CI/CD

The project has been configured with:
- Code quality check
- Build & Test
- Docker image build & push
- Security scan with Trivy
- Automated deployment

## Database Schema

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

## Security

- JWT Bearer Authentication
- Password hashing (SHA256 - should use BCrypt in production)
- Role-based Authorization
- HTTPS enforcement
- CORS configuration
- SQL Injection protection (EF Core parameterized queries)

## Logging

Logs are saved at `logs/log-YYYYMMDD.txt` in JSON format:

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

## Contributing

1. Fork repository
2. Create feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Create a Pull Request

## License

This project is licensed under the MIT License.

## Author

Your Name - [your.email@example.com](mailto:your.email@example.com)

## Acknowledgments

- Clean Architecture by Jason Taylor
- CQRS Pattern
- Domain-Driven Design
- .NET Community
