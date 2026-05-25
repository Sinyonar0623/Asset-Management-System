using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Security;
using System.Security.Claims;

namespace Auth.Authentication.Features.AssignHOD;

public sealed class AssignHODEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/auth/hod", async (
                AssignHODRequest request,
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

                var result = await sender.Send(new AssignHODCommand(request.UserId), cancellationToken);
                var response = new AssignHODResponse(result.Hod);

                return Results.Ok(response);
            })
            .WithName("AssignHOD")
            .Produces<AssignHODResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithTags("Auth")
            .WithSummary("Assign HOD")
            .RequireAuthorization();
    }
}
