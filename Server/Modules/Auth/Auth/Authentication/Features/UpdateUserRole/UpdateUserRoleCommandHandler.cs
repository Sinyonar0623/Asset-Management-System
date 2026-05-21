using Auth.Service;
using Shared.CQRS;

namespace Auth.Authentication.Features.UpdateUserRole;

public sealed class UpdateUserRoleCommandHandler(IAuthService service)
    : ICommandHandler<UpdateUserRoleCommand, UpdateUserRoleResult>
{
    private readonly IAuthService _service = service;

    public async Task<UpdateUserRoleResult> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _service.UpdateUserRole(request.UserId, request.RoleCode, cancellationToken);

        return new UpdateUserRoleResult(user);
    }
}
