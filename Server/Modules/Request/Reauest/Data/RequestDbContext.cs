using Microsoft.EntityFrameworkCore;
using Reauest.Requests.Model;
using Shared.Data.Extensions;
using System.Reflection;

namespace Reauest.Data;

public class RequestDbContext(DbContextOptions<RequestDbContext> options) : DbContext(options)
{
    public DbSet<BorrowRequest> BorrowRequests => Set<BorrowRequest>();
    public DbSet<RepairRequest> RepairRequests => Set<RepairRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("request");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.ApplyAuditConventions();

        base.OnModelCreating(modelBuilder);
    }
}
