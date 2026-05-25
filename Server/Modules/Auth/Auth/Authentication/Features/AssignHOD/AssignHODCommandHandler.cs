using Auth.Service;
using Shared.CQRS;

namespace Auth.Authentication.Features.AssignHOD;

public sealed class AssignHODCommandHandler(IAuthService service)
    : ICommandHandler<AssignHODCommand, AssignHODResult>
{
    private readonly IAuthService _service = service;

    public async Task<AssignHODResult> Handle(AssignHODCommand request, CancellationToken cancellationToken)
    {
        var hod = await _service.AssignHOD(request.UserId, cancellationToken);

        return new AssignHODResult(hod);
    }
}
