using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Security;
using System.Security.Claims;

namespace Asset.Assets.Features.LaboratoryFeature.AssignTeacher;

public class AssignTeacherEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/Laboratory/{id:guid}/Teacher",
            async (
                Guid id,
                AssignTeacherRequest request,
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

                var result = await sender.Send(new AssignTeacherCommand(id, request.TeacherId), cancellationToken);

                var response = new AssignTeacherResponse(result.IsSuccess);

                return Results.Ok(response);
            })
            .WithName("AssignTeacherToLaboratory")
            .Produces<AssignTeacherResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Assign Teacher To Laboratory")
            .RequireAuthorization();
    }
}
