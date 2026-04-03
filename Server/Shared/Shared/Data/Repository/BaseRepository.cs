using Microsoft.EntityFrameworkCore;
using Shared.DDD;

namespace Shared.Data
{
    /// <summary>
    /// Base implementation of a repository.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <typeparam name="TId">The entity ID type.</typeparam>
    public abstract class BaseRepository<T, TId> : BaseReadRepository<T, TId>, IRepository<T, TId>
        where T : class, IEntity<TId>
    {
        protected BaseRepository(DbContext context) : base(context)
        {
        }

        public override async Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        {
            return await GetTrackedQuery().FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
        }

        // Write operations
        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity, cancellationToken);
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
        }

        public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            DbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            DbSet.UpdateRange(entities);
            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            DbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(TId id, CancellationToken cancellationToken = default)
        {
            var entity = await DbSet.FindAsync(new object[] { id }, cancellationToken);
            if (entity != null)
            {
                DbSet.Remove(entity);
            }
        }

        public virtual Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            DbSet.RemoveRange(entities);
            return Task.CompletedTask;
        }
    }
}
