using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Security;
using System.Security.Claims;

namespace Auth.Authentication.Features.UpdateUserRole;

public sealed class UpdateUserRoleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/auth/users/{id:guid}/role", async (
                Guid id,
                UpdateUserRoleRequest request,
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

                var result = await sender.Send(new UpdateUserRoleCommand(id, request.RoleCode), cancellationToken);
                var response = new UpdateUserRoleResponse(result.User);

                return Results.Ok(response);
            })
            .WithName("UpdateUserRole")
            .Produces<UpdateUserRoleResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithTags("Auth")
            .WithSummary("Update User Role")
            .RequireAuthorization();
    }
}
