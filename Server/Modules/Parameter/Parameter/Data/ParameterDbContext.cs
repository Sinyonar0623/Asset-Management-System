using Microsoft.EntityFrameworkCore;
using Shared.Data.Extensions;
using System.Reflection;

namespace Parameter.Data;

public class ParameterDbContext(DbContextOptions<ParameterDbContext> options) : DbContext(options)
{
    public DbSet<Parameters.Model.Parameter> Parameter => Set<Parameters.Model.Parameter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("parameter");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.ApplyAuditConventions();

        base.OnModelCreating(modelBuilder);
    }
}
