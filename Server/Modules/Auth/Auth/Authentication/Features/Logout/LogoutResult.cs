namespace Auth.Authentication.Features.Logout;

public sealed record LogoutResult(
    bool Succeeded,
    string? Error
);
