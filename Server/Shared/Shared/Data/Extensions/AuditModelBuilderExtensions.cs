using Microsoft.EntityFrameworkCore;
using Shared.DDD;

namespace Shared.Data.Extensions;

public static class AuditModelBuilderExtensions
{
    private const string DefaultActor = "SYSTEM";

    public static ModelBuilder ApplyAuditConventions(this ModelBuilder modelBuilder)
    {
        var auditableEntityTypes = modelBuilder.Model
            .GetEntityTypes()
            .Where(t => t.ClrType is not null
                && !t.IsOwned()
                && typeof(IEntity).IsAssignableFrom(t.ClrType))
            .ToList();

        foreach (var entityType in auditableEntityTypes)
        {
            var builder = modelBuilder.Entity(entityType.ClrType);

            builder.Property(nameof(IEntity.CreateOn))
                .HasDefaultValueSql("SYSUTCDATETIME()")
                .ValueGeneratedOnAdd();

            builder.Property(nameof(IEntity.CreateBy))
                .HasDefaultValue(DefaultActor)
                .IsRequired();

            builder.Property(nameof(IEntity.UpdateOn))
                .IsRequired(false);

            builder.Property(nameof(IEntity.UpdateBy))
                .IsRequired(false);
        }

        return modelBuilder;
    }
}
