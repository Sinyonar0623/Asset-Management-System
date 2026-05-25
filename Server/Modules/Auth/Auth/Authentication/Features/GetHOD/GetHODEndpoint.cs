using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Security;
using System.Security.Claims;

namespace Auth.Authentication.Features.GetHOD;

public sealed class GetHODEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/hod", async (
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

                var result = await sender.Send(new GetHODQuery(), cancellationToken);
                var response = new GetHODResponse(result.Hod);

                return Results.Ok(response);
            })
            .WithName("GetHOD")
            .Produces<GetHODResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithTags("Auth")
            .WithSummary("Get HOD")
            .RequireAuthorization();
    }
}
