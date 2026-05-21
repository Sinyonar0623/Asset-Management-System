using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Asset.Assets.Features.AssetFeature.GetAssetHistory;

public class GetAssetHistoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Asset/{id:guid}/history", async (
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

                var result = await sender.Send(
                    new GetAssetHistoryQuery(id, userId, roleCode),
                    cancellationToken);
                var response = new GetAssetHistoryResponse(result.Histories);

                return Results.Ok(response);
            })
            .WithName("GetAssetHistory")
            .Produces<GetAssetHistoryResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Asset History By Asset Id")
            .RequireAuthorization();
    }
}
