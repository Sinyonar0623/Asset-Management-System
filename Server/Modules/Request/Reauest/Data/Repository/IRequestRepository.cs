using Reauest.Requests.Model;

namespace Reauest.Data.Repository;

public interface IRequestRepository
{
    // Borrow
    Task<BorrowRequest?> GetBorrowByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<BorrowRequest> Items, int TotalCount)> GetBorrowsPagedAsync(
        int page, int pageSize, Guid? requesterId, string? status, CancellationToken cancellationToken = default);
    Task AddBorrowAsync(BorrowRequest request, CancellationToken cancellationToken = default);
    Task UpdateBorrowAsync(BorrowRequest request, CancellationToken cancellationToken = default);

    // Repair
    Task<RepairRequest?> GetRepairByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<RepairRequest> Items, int TotalCount)> GetRepairsPagedAsync(
        int page, int pageSize, Guid? requesterId, string? status, CancellationToken cancellationToken = default);
    Task AddRepairAsync(RepairRequest request, CancellationToken cancellationToken = default);
    Task UpdateRepairAsync(RepairRequest request, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
