using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Auth.Authentication.Features.Me;

public sealed class MeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/me", (ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = user.FindFirstValue(ClaimTypes.Name) ?? user.FindFirstValue("unique_name");
            var email = user.FindFirstValue(ClaimTypes.Email);
            var roleCode = user.FindFirstValue("role_code");
            var roleName = user.FindFirstValue(ClaimTypes.Role);

            return Results.Ok(new
            {
                UserId = userId,
                Username = username,
                Email = email,
                RoleCode = roleCode,
                RoleName = roleName
            });
        })
        .WithName("GetMe")
        .WithTags("Auth")
        .RequireAuthorization();
    }
}
