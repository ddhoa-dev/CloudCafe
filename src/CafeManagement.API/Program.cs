using System.Text;
using CafeManagement.API.Middlewares;
using CafeManagement.Application;
using CafeManagement.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Fix PostgreSQL DateTime.Now (Local time) exception
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// ===== SERILOG CONFIGURATION =====
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// ===== ADD SERVICES TO CONTAINER =====

// Application Layer
builder.Services.AddApplication();

// Infrastructure Layer
builder.Services.AddInfrastructure(builder.Configuration);

// Controllers
builder.Services.AddControllers();

// HttpContextAccessor (cần cho CurrentUserService)
builder.Services.AddHttpContextAccessor();

// ===== JWT AUTHENTICATION =====
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero  // Không cho phép token hết hạn vẫn dùng được
    };
});

builder.Services.AddAuthorization();

// ===== SWAGGER/OPENAPI =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cafe Management API",
        Version = "v1",
        Description = "Cloud-Native Cafe Management & Inventory System API",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your.email@example.com"
        }
    });

    // JWT Authentication trong Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ===== BUILD APP =====
var app = builder.Build();

// Tự động tạo Database nếu chưa tồn tại (giải quyết lỗi thiếu bảng Users)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CafeManagement.Infrastructure.Data.ApplicationDbContext>();
    context.Database.EnsureCreated();
    
    // Tự động tạo dữ liệu mẫu nếu chưa có
    await CafeManagement.Infrastructure.Data.DataSeeder.SeedDataAsync(context);
}

// ===== CONFIGURE MIDDLEWARE PIPELINE =====

// Exception Handling Middleware (phải đặt đầu tiên)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Serilog Request Logging
app.UseSerilogRequestLogging();

// Swagger (Development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cafe Management API v1");
        c.RoutePrefix = string.Empty;  // Swagger UI tại root URL
    });
}

// HTTPS Redirection
app.UseHttpsRedirection();

// CORS
app.UseCors("AllowAll");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// ===== RUN APP =====
try
{
    Log.Information("Starting Cafe Management API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
