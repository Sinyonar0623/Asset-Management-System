using Auth.Service;
using Shared.CQRS;

namespace Auth.Authentication.Features.Logout;

public sealed class LogoutCommandHandler(IAuthService authService) : ICommandHandler<LogoutCommand, LogoutResult>
{
    private readonly IAuthService _authService = authService;

    public async Task<LogoutResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var success = await _authService.LogoutAsync(request.UserId, cancellationToken);
        return success
            ? new LogoutResult(true, null)
            : new LogoutResult(false, "User not found.");
    }
}
