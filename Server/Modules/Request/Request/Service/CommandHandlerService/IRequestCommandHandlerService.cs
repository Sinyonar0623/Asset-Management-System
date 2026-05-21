namespace Request.Service.CommandHandlerService;

public interface IRequestCommandHandlerService
{
    Task<Guid> CreateRequest(
        CreateRequestDto request,
        Guid requesterId,
        string requesterRoleCode,
        CancellationToken cancellationToken);
    Task<RequestDto> GetRequestById(Guid requestId, CancellationToken cancellationToken = default);
    Task<RequestDto> GetVisibleRequestById(
        Guid requestId,
        Guid userId,
        string roleCode,
        CancellationToken cancellationToken = default);
    Task<PaginatedResult<RequestDto>> GetRequests(PaginationRequest paginationRequest, CancellationToken cancellationToken = default);
    Task<PaginatedResult<RequestDto>> GetVisibleRequests(
        PaginationRequest paginationRequest,
        Guid userId,
        string roleCode,
        CancellationToken cancellationToken = default);
    Task<bool> UpdateRequest(Guid requestId, UpdateRequestDto request, CancellationToken cancellationToken);
    Task<bool> DeleteRequest(Guid requestId, CancellationToken cancellationToken);
    Task<bool> MarkRequestProcessed(
        Guid requestId,
        Guid approverId,
        string approverRoleCode,
        string decision,
        string? comment,
        List<Guid> assetIds,
        CancellationToken cancellationToken);
    Task<bool> MarkRequestApproved(Guid requestId, CancellationToken cancellationToken);
    Task<bool> ReserveAssetsFromRequest(Guid requestId, Guid approverId, List<Guid> assetIds, CancellationToken cancellationToken);
    Task<bool> ReleaseAssetsFromRequest(Guid requestId, Guid approverId, List<Guid> assetIds, CancellationToken cancellationToken);
    Task<bool> AssignAssetsToLaboratoryFromRequest(
        Guid requestId,
        Guid approverId,
        Guid laboratoryId,
        List<Guid> assetIds,
        CancellationToken cancellationToken);
    Task<bool> MarkAssetsInUseFromRequest(
        Guid requestId,
        Guid approverId,
        Guid responsibleUserId,
        List<Guid> assetIds,
        CancellationToken cancellationToken);
    Task<bool> ReassignHODApprover(
        Guid oldHODApproverId,
        Guid newHODApproverId,
        CancellationToken cancellationToken);
    Task<int> CompleteExpiredBorrowRequests(DateTime expiredBeforeUtc, int batchSize, CancellationToken cancellationToken);
}
