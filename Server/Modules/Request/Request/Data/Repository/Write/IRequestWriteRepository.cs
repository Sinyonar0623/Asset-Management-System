using Shared.Data;

namespace Request.Data.Repository.Write;

public interface IRequestWriteRepository : IRepository<Requests.Model.Request, Guid>
{
    Task<List<Requests.Model.Request>> GetPendingRequestsAssignedToHODAsync(
        Guid hodApproverId,
        CancellationToken cancellationToken = default);
    Task<List<Requests.Model.Request>> GetApprovedBorrowRequestsDueForCompletionAsync(
        DateTime expiredBeforeUtc,
        int batchSize,
        CancellationToken cancellationToken = default);
}
