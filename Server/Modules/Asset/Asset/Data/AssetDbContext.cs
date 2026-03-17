using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Shared.Data.Extensions;
using Shared.Data.Outbox.Configuration;
using Shared.Outbox.Model;

namespace Asset.Data;

public class AssetDbContext(DbContextOptions<AssetDbContext> options) : DbContext(options)
{
    public DbSet<Assets.Model.Laboratory> Laboratories => Set<Assets.Model.Laboratory>();
    public DbSet<Assets.Model.Asset> AssetModels => Set<Assets.Model.Asset>();
    public DbSet<Assets.Model.AssetUnit> AssetUnits => Set<Assets.Model.AssetUnit>();
    public DbSet<Assets.Model.AssetUnitImage> AssetUnitImages => Set<Assets.Model.AssetUnitImage>();
    public DbSet<Assets.Model.AssetUnitCondition> AssetUnitConditions => Set<Assets.Model.AssetUnitCondition>();
    public DbSet<Outbox> OutboxMessages => Set<Outbox>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("asset");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.ApplyConfiguration(new OutboxConfiguration(excludeFromMigrations: false));
        modelBuilder.ApplyAuditConventions();

        base.OnModelCreating(modelBuilder);
    }
}
