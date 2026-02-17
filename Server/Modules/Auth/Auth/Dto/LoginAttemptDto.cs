namespace Auth.Dto;

public sealed record LoginAttemptDto(
    bool Succeeded,
    LoginUserDto? User,
    string? Error,
    bool IsValidationError,
    bool IsAlreadyLoggedIn
);
