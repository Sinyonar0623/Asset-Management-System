using Auth.Service;
using Shared.CQRS;

namespace Auth.Authentication.Features.GetUsers;

public sealed class GetUsersQueryHandler(IAuthService service)
    : IQueryHandler<GetUsersQuery, GetUsersResult>
{
    private readonly IAuthService _service = service;

    public async Task<GetUsersResult> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _service.GetUsers(cancellationToken);

        return new GetUsersResult(users);
    }
}
