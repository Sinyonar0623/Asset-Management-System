namespace Auth.Authentication.Features.Login;

public sealed record LoginResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    Guid UserId,
    string Username,
    string Email,
    string RoleCode,
    string RoleName
);
