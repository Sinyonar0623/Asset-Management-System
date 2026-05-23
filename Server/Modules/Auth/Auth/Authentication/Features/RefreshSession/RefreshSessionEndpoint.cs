using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.Authentication.Features.Login;
using Auth.Authentication.Jwt;
using Auth.Service;
using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Auth.Authentication.Features.RefreshSession;

public sealed class RefreshSessionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/refresh", async (
                HttpContext httpContext,
                IAuthService authService,
                IJwtTokenGenerator jwtTokenGenerator,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var refreshAttempt = await authService.RefreshSessionAsync(userId, cancellationToken);
                if (!refreshAttempt.Succeeded || refreshAttempt.User is null)
                {
                    return Results.Unauthorized();
                }

                var user = refreshAttempt.User;
                var (accessToken, expiresInSeconds) = jwtTokenGenerator.GenerateToken(user);
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

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("RefreshSession")
            .WithTags("Auth");
    }
}
