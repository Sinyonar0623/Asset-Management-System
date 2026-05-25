using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Security;
using System.Security.Claims;

namespace Asset.Assets.Features.AssetFeature.ReturnAsset;

public sealed class ReturnAssetEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/Asset/{id:guid}/return", async (
                Guid id,
                HttpContext httpContext,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? httpContext.User.FindFirstValue("sub");

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var roleCode = httpContext.User.FindFirstValue("role_code");
                if (string.IsNullOrWhiteSpace(roleCode))
                {
                    return Results.Unauthorized();
                }

                var normalizedRoleCode = roleCode.Trim().ToUpperInvariant();
                if (normalizedRoleCode is not (RoleCodes.Student or RoleCodes.Teacher or RoleCodes.Hod or RoleCodes.Admin))
                {
                    return Results.Forbid();
                }

                ReturnAssetResult result;
                try
                {
                    result = await sender.Send(
                        new ReturnAssetCommand(id, userId, normalizedRoleCode),
                        cancellationToken);
                }
                catch (KeyNotFoundException)
                {
                    return Results.NotFound();
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Forbid();
                }

                var response = new ReturnAssetResponse(result.IsSuccess);

                return Results.Ok(response);
            })
            .WithName("ReturnAsset")
            .Produces<ReturnAssetResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Return Asset")
            .RequireAuthorization();
    }
}
