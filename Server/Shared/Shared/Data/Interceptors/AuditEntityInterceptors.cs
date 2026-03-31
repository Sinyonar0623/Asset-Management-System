using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.Data.Audit;
using Shared.DDD;

namespace Shared.Data.Interceptors;

public class AuditEntityInterceptors(ICurrentActorProvider currentActorProvider) : SaveChangesInterceptor
{
    private const string DefaultActor = "SYSTEM";
    private readonly ICurrentActorProvider _currentActorProvider = currentActorProvider;

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAudit(DbContext? context)
    {
        if (context is null) return;

        var utcNow = DateTime.UtcNow;
        var actor = _currentActorProvider.GetCurrentActor();
        if (string.IsNullOrWhiteSpace(actor))
        {
            actor = DefaultActor;
        }

        foreach (var entry in context.ChangeTracker.Entries<IEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.CreateOn is null)
                {
                    entry.Entity.CreateOn = utcNow;
                }

                if (string.IsNullOrWhiteSpace(entry.Entity.CreateBy))
                {
                    entry.Entity.CreateBy = actor;
                }
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(IEntity.CreateOn)).IsModified = false;
                entry.Property(nameof(IEntity.CreateBy)).IsModified = false;

                entry.Entity.UpdateOn = utcNow;

                if (string.IsNullOrWhiteSpace(entry.Entity.UpdateBy))
                {
                    entry.Entity.UpdateBy = actor;
                }
            }
        }
    }
}
