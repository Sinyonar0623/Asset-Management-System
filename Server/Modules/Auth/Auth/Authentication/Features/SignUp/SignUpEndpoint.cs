using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Security;
using System.Security.Claims;

namespace Auth.Authentication.Features.SignUp;

public sealed class SignUpEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/signup/user", async (
                SignUpRequest request,
                HttpContext httpContext,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var roleCode = httpContext.User.FindFirstValue("role_code");
                if (string.IsNullOrWhiteSpace(roleCode))
                {
                    return Results.Unauthorized();
                }

                if (!string.Equals(roleCode, RoleCodes.Admin, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.Forbid();
                }

                var result = await sender.Send(
                    new SignUpCommand(request.Username, request.Email, request.Password, request.RoleCode),
                    cancellationToken);

                if (!result.Succeeded || result.UserId is null)
                {
                    return Results.BadRequest(new { message = result.Error });
                }

                return Results.Created($"/auth/users/{result.UserId}", new SignUpResponse(result.UserId.Value));
            })
            .WithName("SignUp")
            .Produces<SignUpResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithTags("Auth")
            .RequireAuthorization();
    }
}
