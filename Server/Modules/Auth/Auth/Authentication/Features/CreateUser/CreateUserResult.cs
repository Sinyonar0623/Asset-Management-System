namespace Auth.Authentication.Features.CreateUser;

public sealed record CreateUserResult(
    bool Succeeded,
    Guid? UserId,
    string? Error
);
