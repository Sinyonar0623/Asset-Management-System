using System.Reflection;
using Auth.Authentication.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Data.Extensions;

namespace Auth.Data;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<UserName> UserName => Set<UserName>();
    public DbSet<UserRole> UserRole => Set<UserRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("auth");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.ApplyAuditConventions();

        base.OnModelCreating(modelBuilder);
    }
}
