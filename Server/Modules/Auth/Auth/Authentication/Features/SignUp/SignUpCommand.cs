using Shared.CQRS;

namespace Auth.Authentication.Features.SignUp;

public sealed record SignUpCommand(
    string Username,
    string Email,
    string Password,
    string RoleCode
) : ICommand<SignUpResult>;
