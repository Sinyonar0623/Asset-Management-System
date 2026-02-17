using Shared.CQRS;

namespace Auth.Authentication.Features.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : ICommand<LoginResult>;

