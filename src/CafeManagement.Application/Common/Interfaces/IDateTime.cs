namespace CafeManagement.Application.Common.Interfaces;

/// <summary>
/// DateTime abstraction để dễ dàng test
/// </summary>
public interface IDateTime
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}
