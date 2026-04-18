namespace Request.Service.CommandHandlerService;

public interface IRequestCommandHandlerService
{
    Task<Guid> CreateRequest(CreateRequestDto request, Guid requesterId, CancellationToken cancellationToken);
    Task<RequestDto> GetRequestById(Guid requestId, CancellationToken cancellationToken = default);
    Task<PaginatedResult<RequestDto>> GetRequests(PaginationRequest paginationRequest, CancellationToken cancellationToken = default);
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
}
