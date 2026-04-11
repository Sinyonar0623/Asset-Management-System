namespace Request.Service.CommandHandlerService;

public interface IRequestCommandHandlerService
{
    Task<Guid> CreateRequest(CreateRequestDto request, CancellationToken cancellationToken);
    Task<RequestDto> GetRequestById(Guid requestId, CancellationToken cancellationToken = default);
    Task<PaginatedResult<RequestDto>> GetRequests(PaginationRequest paginationRequest, CancellationToken cancellationToken = default);
    Task<bool> UpdateRequest(Guid requestId, UpdateRequestDto request, CancellationToken cancellationToken);
    Task<bool> DeleteRequest(Guid requestId, CancellationToken cancellationToken);
}
