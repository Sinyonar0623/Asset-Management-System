using Request.Service.CommandHandlerService;

namespace Request.Requests.Features.Request.GetRequestById;

public class GetRequestByIdQueryHandler(IRequestCommandHandlerService service)
    : IQueryHandler<GetRequestByIdQuery, GetRequestByIdResult>
{
    private readonly IRequestCommandHandlerService _service = service;

    public async Task<GetRequestByIdResult> Handle(GetRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var requestById = await _service.GetRequestById(request.Id, cancellationToken);
        return new GetRequestByIdResult(requestById);
    }
}
