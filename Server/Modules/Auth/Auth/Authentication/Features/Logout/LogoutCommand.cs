using Shared.CQRS;

namespace Auth.Authentication.Features.Logout;

public sealed record LogoutCommand(Guid UserId) : ICommand<LogoutResult>;
