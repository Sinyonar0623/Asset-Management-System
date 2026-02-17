using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Auth.Authentication.Features.Logout;

public sealed class LogoutEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/logout", async (HttpContext httpContext, ISender sender, CancellationToken cancellationToken) =>
            {
                var userIdClaim = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var result = await sender.Send(new LogoutCommand(userId), cancellationToken);
                if (!result.Succeeded)
                {
                    return Results.NotFound(new { message = result.Error });
                }

                return Results.Ok(new { message = "Logged out successfully." });
            })
            .RequireAuthorization()
            .WithName("Logout")
            .WithTags("Auth");
    }
}
