using System.Net;
using System.Text.Json;
using CafeManagement.Domain.Exceptions;
using FluentValidation;

namespace CafeManagement.API.Middlewares;

/// <summary>
/// Global Exception Handler Middleware
/// Bắt tất cả exceptions và trả về response thống nhất
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Tiếp tục pipeline
            await _next(context);
        }
        catch (Exception ex)
        {
            // Bắt exception và xử lý
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Xử lý exception và trả về response phù hợp
    /// </summary>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Log exception
        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        // Prepare response
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            // ===== VALIDATION EXCEPTION (FluentValidation) =====
            ValidationException validationException => new
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Validation failed",
                Errors = (object)validationException.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Message = e.ErrorMessage
                }).ToArray()
            },

            // ===== NOT FOUND EXCEPTION =====
            NotFoundException notFoundException => new
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = notFoundException.Message,
                Errors = (object)Array.Empty<object>()
            },

            // ===== INSUFFICIENT STOCK EXCEPTION =====
            InsufficientStockException insufficientStockException => new
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = insufficientStockException.Message,
                Errors = (object)new[]
                {
                    new
                    {
                        Property = "Inventory",
                        Message = $"Nguyên liệu '{insufficientStockException.IngredientName}' không đủ. " +
                                 $"Cần: {insufficientStockException.RequiredQuantity}, " +
                                 $"Còn: {insufficientStockException.AvailableQuantity}"
                    }
                }
            },

            // ===== DOMAIN EXCEPTION =====
            DomainException domainException => new
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = domainException.Message,
                Errors = (object)Array.Empty<object>()
            },

            // ===== UNHANDLED EXCEPTION =====
            _ => new
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An internal server error occurred",
                Errors = (object)Array.Empty<object>()
            }
        };

        context.Response.StatusCode = response.StatusCode;

        // Serialize và trả về JSON
        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
