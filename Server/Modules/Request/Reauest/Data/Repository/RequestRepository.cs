using Microsoft.EntityFrameworkCore;
using Reauest.Requests.Model;

namespace Reauest.Data.Repository;

public class RequestRepository(RequestDbContext dbContext) : IRequestRepository
{
    private readonly RequestDbContext _context = dbContext;

    public async Task<BorrowRequest?> GetBorrowByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.BorrowRequests.FindAsync([id], cancellationToken);
    }

    public async Task<(IEnumerable<BorrowRequest> Items, int TotalCount)> GetBorrowsPagedAsync(
        int page, int pageSize, Guid? requesterId, string? status, CancellationToken cancellationToken = default)
    {
        var query = _context.BorrowRequests.AsNoTracking().AsQueryable();

        if (requesterId.HasValue)
            query = query.Where(b => b.RequesterId == requesterId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(b => b.Status == status);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(b => b.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task AddBorrowAsync(BorrowRequest request, CancellationToken cancellationToken = default)
    {
        await _context.BorrowRequests.AddAsync(request, cancellationToken);
    }

    public Task UpdateBorrowAsync(BorrowRequest request, CancellationToken cancellationToken = default)
    {
        _context.BorrowRequests.Update(request);
        return Task.CompletedTask;
    }

    public async Task<RepairRequest?> GetRepairByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.RepairRequests.FindAsync([id], cancellationToken);
    }

    public async Task<(IEnumerable<RepairRequest> Items, int TotalCount)> GetRepairsPagedAsync(
        int page, int pageSize, Guid? requesterId, string? status, CancellationToken cancellationToken = default)
    {
        var query = _context.RepairRequests.AsNoTracking().AsQueryable();

        if (requesterId.HasValue)
            query = query.Where(r => r.RequesterId == requesterId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(r => r.Status == status);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(r => r.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task AddRepairAsync(RepairRequest request, CancellationToken cancellationToken = default)
    {
        await _context.RepairRequests.AddAsync(request, cancellationToken);
    }

    public Task UpdateRepairAsync(RepairRequest request, CancellationToken cancellationToken = default)
    {
        _context.RepairRequests.Update(request);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
