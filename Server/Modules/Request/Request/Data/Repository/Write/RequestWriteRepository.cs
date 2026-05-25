using Microsoft.EntityFrameworkCore;
using Request.Requests.Model;
using Shared.Data;
using Shared.Security;

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

    public async Task<List<Requests.Model.Request>> GetPendingRequestsAssignedToHODAsync(
        Guid hodApproverId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Requests
            .Include(x => x.Detail)
            .Include(x => x.Items)
            .Include(x => x.Trackings)
            .Where(x =>
                x.Status == RequestStatusCodes.Pending
                && x.Trackings.Any(t =>
                    t.AssignedApproverId == hodApproverId
                    && t.RequiredRoleCode == RoleCodes.Hod
                    && (t.Status == TrackingStatusCodes.Waiting || t.Status == TrackingStatusCodes.Pending)))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Requests.Model.Request>> GetApprovedBorrowRequestsDueForCompletionAsync(
        DateTime expiredBeforeUtc,
        int batchSize,
        CancellationToken cancellationToken = default)
    {
        return await _context.Requests
            .Include(x => x.Detail)
            .Include(x => x.Items)
            .Include(x => x.Trackings)
            .Where(x =>
                x.RequestType == RequestTypeCodes.Borrow
                && x.Status == RequestStatusCodes.Approved
                && x.Detail != null
                && x.Detail.BorrowTo.HasValue
                && x.Detail.BorrowTo.Value < expiredBeforeUtc)
            .OrderBy(x => x.Detail!.BorrowTo)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }
}
