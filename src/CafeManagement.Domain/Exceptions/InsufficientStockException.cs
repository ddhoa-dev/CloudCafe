namespace CafeManagement.Domain.Exceptions;

public class InsufficientStockException : DomainException
{
    public string IngredientName { get; }
    public decimal RequiredQuantity { get; }
    public decimal AvailableQuantity { get; }

    public InsufficientStockException(
        string ingredientName,
        decimal requiredQuantity,
        decimal availableQuantity)
        : base($"Không đủ nguyên liệu '{ingredientName}'. Cần: {requiredQuantity}, Còn: {availableQuantity}")
    {
        IngredientName = ingredientName;
        RequiredQuantity = requiredQuantity;
        AvailableQuantity = availableQuantity;
    }
}
