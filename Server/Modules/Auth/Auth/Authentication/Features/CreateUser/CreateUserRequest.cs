namespace Auth.Authentication.Features.CreateUser;

public sealed record CreateUserRequest(
    string Username,
    string Email,
    string Password,
    string RoleCode
);
