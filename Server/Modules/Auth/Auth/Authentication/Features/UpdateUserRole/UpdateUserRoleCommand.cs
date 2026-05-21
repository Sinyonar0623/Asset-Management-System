using Shared.CQRS;

namespace Auth.Authentication.Features.UpdateUserRole;

public record UpdateUserRoleCommand(Guid UserId, string RoleCode) : ICommand<UpdateUserRoleResult>;
