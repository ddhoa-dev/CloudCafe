using CafeManagement.Application.Common.Interfaces;
using CafeManagement.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CafeManagement.Infrastructure.Data.Interceptors;

/// <summary>
/// Interceptor để tự động set Auditing fields (CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
/// Chạy trước khi SaveChanges() được gọi
/// </summary>
public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;

    public AuditableEntityInterceptor(
        ICurrentUserService currentUserService,
        IDateTime dateTime)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    /// <summary>
    /// Hook vào trước khi SaveChanges() được gọi
    /// Tự động set CreatedAt, CreatedBy cho entities mới
    /// Tự động set LastModifiedAt, LastModifiedBy cho entities đã sửa
    /// </summary>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Async version của SavingChanges
    /// </summary>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Update Auditing fields cho các entities
    /// </summary>
    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        // Lấy tất cả entities đang được track bởi EF Core
        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            // ===== ENTITY MỚI (Added) =====
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = _currentUserService.UserId ?? "System";
                entry.Entity.CreatedAt = _dateTime.Now;
            }

            // ===== ENTITY ĐÃ SỬA (Modified) =====
            if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.LastModifiedBy = _currentUserService.UserId ?? "System";
                entry.Entity.LastModifiedAt = _dateTime.Now;
            }
        }
    }
}

/// <summary>
/// Extension methods cho EntityEntry
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Kiểm tra entity có owned entities đã thay đổi không
    /// </summary>
    public static bool HasChangedOwnedEntities(this Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
    {
        return entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
    }
}
