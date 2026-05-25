using Auth.Service;
using Shared.CQRS;

namespace Auth.Authentication.Features.GetUserLookup;

public sealed class GetUserLookupQueryHandler(IAuthService service)
    : IQueryHandler<GetUserLookupQuery, GetUserLookupResult>
{
    private readonly IAuthService _service = service;

    public async Task<GetUserLookupResult> Handle(
        GetUserLookupQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _service.GetUserLookup(request.UserIds, cancellationToken);

        return new GetUserLookupResult(users);
    }
}
