using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Shared.Data.Outbox.Configuration;
using Shared.Outbox.Model;

namespace Asset.Data;

public class AssetDbContext(DbContextOptions<AssetDbContext> options) : DbContext(options)
{
    public DbSet<Assets.Model.Asset> Assets => Set<Assets.Model.Asset>();
    public DbSet<Outbox> OutboxMessages => Set<Outbox>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("asset");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.ApplyConfiguration(new OutboxConfiguration(excludeFromMigrations: false));

        base.OnModelCreating(modelBuilder);
    }
}