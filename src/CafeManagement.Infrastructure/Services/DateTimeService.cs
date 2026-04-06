using CafeManagement.Application.Common.Interfaces;

namespace CafeManagement.Infrastructure.Services;

/// <summary>
/// Implementation của IDateTime
/// Dùng để abstract DateTime.Now để dễ dàng test
/// </summary>
public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}
