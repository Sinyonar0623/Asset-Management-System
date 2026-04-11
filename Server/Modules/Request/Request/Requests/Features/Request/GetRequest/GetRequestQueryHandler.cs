using Request.Service.CommandHandlerService;

namespace Request.Requests.Features.Request.GetRequest;

public class GetRequestQueryHandler(IRequestCommandHandlerService service)
    : IQueryHandler<GetRequestQuery, GetRequestResult>
{
    private readonly IRequestCommandHandlerService _service = service;

    public async Task<GetRequestResult> Handle(GetRequestQuery request, CancellationToken cancellationToken)
    {
        var requests = await _service.GetRequests(request.PaginationRequest, cancellationToken);
        return new GetRequestResult(requests);
    }
}
