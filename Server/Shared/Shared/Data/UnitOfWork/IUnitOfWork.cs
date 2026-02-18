using Microsoft.EntityFrameworkCore;

namespace Shared.Data.UnitOfWork;

public interface IUnitOfWork<TDbContext> where TDbContext : DbContext
{
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
