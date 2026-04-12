using Microsoft.EntityFrameworkCore;
using Shared.Data;

namespace Request.Data.Repository.Write;

public class RequestWriteRepository(RequestDbContext dbContext)
    : BaseRepository<Requests.Model.Request, Guid>(dbContext), IRequestWriteRepository
{
    private readonly RequestDbContext _context = dbContext;

    public override async Task<Requests.Model.Request?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tracked = _context.ChangeTracker
            .Entries<Requests.Model.Request>()
            .FirstOrDefault(e => e.Entity.Id == id)
            ?.Entity;

        if (tracked is not null)
        {
            return tracked;
        }

        return await _context.Requests
            .Include(x => x.Detail)
            .Include(x => x.Items)
            .Include(x => x.Trackings)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
