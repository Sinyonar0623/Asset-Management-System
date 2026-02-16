using Microsoft.EntityFrameworkCore;

namespace Parameter.Data;

public class ParameterDbContext(DbContextOptions<ParameterDbContext> options) : DbContext(options)
{
    public DbSet<Parameters.Model.Parameter> Parameter => Set<Parameters.Model.Parameter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}