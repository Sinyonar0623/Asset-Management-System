using Request.Service.CommandHandlerService;

namespace Request.Requests.Features.Request.GetRequest;

public class GetRequestQueryHandler(IRequestCommandHandlerService service)
    : IQueryHandler<GetRequestQuery, GetRequestResult>
{
    private readonly IRequestCommandHandlerService _service = service;

    public async Task<GetRequestResult> Handle(GetRequestQuery request, CancellationToken cancellationToken)
    {
        var requests = await _service.GetVisibleRequests(
            request.PaginationRequest,
            request.UserId,
            request.RoleCode,
            cancellationToken);

        return new GetRequestResult(requests);
    }
}
