using Shared.CQRS;

namespace Auth.Authentication.Features.AssignHOD;

public record AssignHODCommand(Guid UserId) : ICommand<AssignHODResult>;
