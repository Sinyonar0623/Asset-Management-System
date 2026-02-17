namespace Auth.Authentication.Features.SignUp;

public sealed record SignUpResult(
    bool Succeeded,
    Guid? UserId,
    string? Error
);
