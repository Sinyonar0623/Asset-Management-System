using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Pagination;
using System.Security.Claims;

namespace Asset.Assets.Features.AssetFeature.GetAssetByLab;

public class GetAssetByLabEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Asset/laboratory/{laboratoryId:guid}",
            async (
                Guid laboratoryId,
                [AsParameters] PaginationRequest request,
                [FromQuery] string? search,
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
                    new GetAssetByLabQuery(laboratoryId, request, userId, roleCode, search),
                    cancellationToken);
                var response = new GetAssetByLabResponse(result.Assets);

                return Results.Ok(response);
            })
            .WithName("GetAssetByLab")
            .Produces<GetAssetByLabResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Asset By Laboratory")
            .RequireAuthorization();
    }
}
