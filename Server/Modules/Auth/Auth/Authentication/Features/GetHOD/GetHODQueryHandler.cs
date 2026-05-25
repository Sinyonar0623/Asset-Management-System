using Auth.Service;
using Shared.CQRS;

namespace Auth.Authentication.Features.GetHOD;

public class GetHODQueryHandler(IAuthService service)
    : IQueryHandler<GetHODQuery, GetHODResult>
{
    private readonly IAuthService _service = service;

    public async Task<GetHODResult> Handle(GetHODQuery request, CancellationToken cancellationToken)
    {
        var hod = await _service.GetHOD(cancellationToken);

        return new GetHODResult(hod);
    }
}
