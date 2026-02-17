namespace Auth.Authentication.Features.SignUp;

public sealed record SignUpRequest(
    string Username,
    string Email,
    string Password,
    string RoleCode
);
