using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Pagination;

namespace Asset.Assets.Features.AssetFeature.GetAllocatableAsset;

public class GetAllocatableAssetEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Asset/allocatable",
                async (
                    [AsParameters] PaginationRequest request,
                    [FromQuery] string? search,
                    HttpContext httpContext,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var roleCode = httpContext.User.FindFirst("role_code")?.Value;
                    if (string.IsNullOrWhiteSpace(roleCode))
                    {
                        return Results.Unauthorized();
                    }

                    var result = await sender.Send(new GetAllocatableAssetQuery(request, roleCode, search), cancellationToken);
                    var response = new GetAllocatableAssetResponse(result.Assets);

                    return Results.Ok(response);
                })
            .WithName("GetAllocatableAsset")
            .Produces<GetAllocatableAssetResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithSummary("Get Allocatable Asset")
            .RequireAuthorization();
    }
}
