using Auth.Authentication.Jwt;
using Auth.Service;
using Shared.CQRS;

namespace Auth.Authentication.Features.Login;

public sealed class LoginCommandHandler(
    IAuthService authService,
    IJwtTokenGenerator jwtTokenGenerator)
    : ICommandHandler<LoginCommand, LoginResult>
{
    private readonly IAuthService _authService = authService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new LoginResult(false, null, "Email and password are required.", true, false);
        }

        var loginAttempt = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);
        if (!loginAttempt.Succeeded || loginAttempt.User is null)
        {
            return new LoginResult(
                false,
                null,
                loginAttempt.Error ?? "Invalid email or password.",
                loginAttempt.IsValidationError,
                loginAttempt.IsAlreadyLoggedIn);
        }

        var user = loginAttempt.User;
        var (accessToken, expiresInSeconds) = _jwtTokenGenerator.GenerateToken(user);
        var response = new LoginResponse(
            accessToken,
            "Bearer",
            expiresInSeconds,
            user.UserId,
            user.Username,
            user.Email,
            user.RoleCode,
            user.RoleName
        );

        return new LoginResult(true, response, null, false, false);
    }
}
