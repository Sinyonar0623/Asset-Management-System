using Shared.CQRS;

namespace Auth.Authentication.Features.CreateUser;

public sealed record CreateUserCommand(
    string Username,
    string Email,
    string Password,
    string RoleCode
) : ICommand<CreateUserResult>;
