namespace Auth.Authentication.Features.Login;

public sealed record LoginResult(
    bool Succeeded,
    LoginResponse? Response,
    string? Error,
    bool IsValidationError,
    bool IsAlreadyLoggedIn
);
