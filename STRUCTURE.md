# Cấu trúc dự án Cloud-Native Cafe Management System

## Tổng quan Clean Architecture (4 Layers)

```
CafeManagement/
│
├── src/
│   │
│   ├── CafeManagement.Domain/                    # LAYER 1: Domain (Core)
│   │   ├── Common/
│   │   │   ├── BaseEntity.cs                     # Base class với Auditing
│   │   │   └── IAuditableEntity.cs               # Interface cho Auditing
│   │   ├── Entities/
│   │   │   ├── Product.cs                        # Sản phẩm (Cafe, Trà, etc.)
│   │   │   ├── Ingredient.cs                     # Nguyên liệu (Cafe, Sữa, Đường)
│   │   │   ├── ProductIngredient.cs              # Công thức (Recipe)
│   │   │   ├── Order.cs                          # Đơn hàng
│   │   │   ├── OrderItem.cs                      # Chi tiết đơn hàng
│   │   │   └── User.cs                           # User cho Authentication
│   │   ├── Enums/
│   │   │   ├── OrderStatus.cs                    # Trạng thái đơn hàng
│   │   │   ├── ProductCategory.cs                # Loại sản phẩm
│   │   │   └── UserRole.cs                       # Vai trò người dùng
│   │   ├── Exceptions/
│   │   │   ├── DomainException.cs                # Base exception
│   │   │   ├── NotFoundException.cs              # Không tìm thấy entity
│   │   │   └── InsufficientStockException.cs     # Không đủ nguyên liệu
│   │   ├── Interfaces/
│   │   │   ├── IRepository.cs                    # Generic Repository
│   │   │   └── IUnitOfWork.cs                    # Unit of Work Pattern
│   │   └── CafeManagement.Domain.csproj
│   │
│   ├── CafeManagement.Application/               # LAYER 2: Application (Use Cases)
│   │   ├── Common/
│   │   │   ├── Interfaces/
│   │   │   │   ├── IApplicationDbContext.cs      # DbContext Interface
│   │   │   │   ├── ICurrentUserService.cs        # Lấy thông tin user hiện tại
│   │   │   │   └── IDateTime.cs                  # DateTime abstraction
│   │   │   ├── Mappings/
│   │   │   │   └── MappingProfile.cs             # AutoMapper Profile
│   │   │   └── Models/
│   │   │       └── Result.cs                     # Result Pattern
│   │   ├── DTOs/
│   │   │   ├── Products/
│   │   │   │   ├── ProductDto.cs
│   │   │   │   └── CreateProductDto.cs
│   │   │   ├── Orders/
│   │   │   │   ├── OrderDto.cs
│   │   │   │   └── CreateOrderDto.cs
│   │   │   └── Auth/
│   │   │       ├── LoginDto.cs
│   │   │       └── TokenDto.cs
│   │   ├── Features/
│   │   │   ├── Orders/
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── CreateOrder/
│   │   │   │   │   │   ├── CreateOrderCommand.cs
│   │   │   │   │   │   ├── CreateOrderCommandHandler.cs
│   │   │   │   │   │   └── CreateOrderCommandValidator.cs
│   │   │   │   │   └── UpdateOrderStatus/
│   │   │   │   └── Queries/
│   │   │   │       └── GetOrders/
│   │   │   │           ├── GetOrdersQuery.cs
│   │   │   │           └── GetOrdersQueryHandler.cs
│   │   │   ├── Products/
│   │   │   │   ├── Commands/
│   │   │   │   └── Queries/
│   │   │   └── Ingredients/
│   │   │       ├── Commands/
│   │   │       └── Queries/
│   │   ├── Services/
│   │   │   └── InventoryService.cs               # Logic tự động trừ tồn kho
│   │   └── CafeManagement.Application.csproj
│   │
│   ├── CafeManagement.Infrastructure/            # LAYER 3: Infrastructure
│   │   ├── Data/
│   │   │   ├── ApplicationDbContext.cs           # EF Core DbContext
│   │   │   ├── Configurations/                   # Entity Configurations
│   │   │   │   ├── ProductConfiguration.cs
│   │   │   │   ├── IngredientConfiguration.cs
│   │   │   │   └── OrderConfiguration.cs
│   │   │   ├── Interceptors/
│   │   │   │   └── AuditableEntityInterceptor.cs # Tự động set Auditing fields
│   │   │   └── Migrations/                       # EF Migrations
│   │   ├── Repositories/
│   │   │   ├── Repository.cs                     # Generic Repository Implementation
│   │   │   └── UnitOfWork.cs                     # Unit of Work Implementation
│   │   ├── Services/
│   │   │   ├── DateTimeService.cs                # DateTime Service
│   │   │   └── CurrentUserService.cs             # Current User Service
│   │   ├── Identity/
│   │   │   ├── JwtTokenService.cs                # JWT Token Generation
│   │   │   └── PasswordHasher.cs                 # Password Hashing
│   │   ├── Caching/
│   │   │   └── RedisCacheService.cs              # Redis Caching (Optional)
│   │   └── CafeManagement.Infrastructure.csproj
│   │
│   └── CafeManagement.API/                       # LAYER 4: Presentation (API)
│       ├── Controllers/
│       │   ├── OrdersController.cs
│       │   ├── ProductsController.cs
│       │   ├── IngredientsController.cs
│       │   └── AuthController.cs
│       ├── Middlewares/
│       │   ├── ExceptionHandlingMiddleware.cs    # Global Error Handler
│       │   └── RequestLoggingMiddleware.cs       # Request Logging
│       ├── Extensions/
│       │   ├── ServiceCollectionExtensions.cs    # DI Registration
│       │   └── ApplicationBuilderExtensions.cs   # Middleware Registration
│       ├── Program.cs                            # Entry Point
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── CafeManagement.API.csproj
│
├── tests/
│   ├── CafeManagement.UnitTests/
│   └── CafeManagement.IntegrationTests/
│
├── docker-compose.yml                            # Docker Compose Configuration
├── Dockerfile                                    # Multi-stage Dockerfile
├── .dockerignore
├── .gitignore
├── README.md
├── STRUCTURE.md                                  # File này
└── CafeManagement.sln                            # Solution File

```

## Giải thích các Layer

### 1. Domain Layer (Core)
- Không phụ thuộc vào layer nào khác
- Chứa Business Entities, Domain Logic, Interfaces
- Định nghĩa các Exception riêng cho Domain

### 2. Application Layer
- Phụ thuộc vào Domain Layer
- Chứa Business Logic, Use Cases (CQRS với MediatR)
- DTOs, Validation (FluentValidation), Mapping (AutoMapper)

### 3. Infrastructure Layer
- Phụ thuộc vào Domain và Application Layer
- Triển khai các Interface từ Domain/Application
- DbContext, Repositories, External Services, Caching

### 4. API Layer (Presentation)
- Phụ thuộc vào Application và Infrastructure Layer
- Controllers, Middlewares, DI Configuration
- Entry point của ứng dụng

## Dependency Flow

```
API → Infrastructure → Application → Domain
                    ↓
                Application
```

## Các tính năng đã implement

✅ Clean Architecture với 4 layers rõ ràng
✅ Domain Entities với Auditing (CreatedAt, CreatedBy, etc.)
✅ Repository Pattern & Unit of Work
✅ Domain Exceptions (NotFoundException, InsufficientStockException)
✅ Enums cho OrderStatus, ProductCategory, UserRole
✅ Recipe Engine (ProductIngredient) để quản lý công thức

## Tiếp theo cần làm

- [ ] Application Layer: CQRS Commands/Queries với MediatR
- [ ] Infrastructure Layer: DbContext, Configurations, Migrations
- [ ] API Layer: Controllers, Middlewares, JWT Authentication
- [ ] DevOps: Dockerfile, docker-compose.yml, GitHub Actions
- [ ] Logging: Serilog configuration
