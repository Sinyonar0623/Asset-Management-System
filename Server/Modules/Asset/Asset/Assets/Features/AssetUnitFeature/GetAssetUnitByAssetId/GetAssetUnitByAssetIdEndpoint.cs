using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.AssetUnitFeature.GetAssetUnitByAssetId;

public class GetAssetUnitByAssetIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/AssetUnit/asset/{assetId:guid}", async (Guid assetId, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAssetUnitByAssetIdQuery(assetId), cancellationToken);
                var response = new GetAssetUnitByAssetIdResponse(result.AssetUnits);

                return Results.Ok(response);
            })
            .WithName("GetAssetUnitsByAssetId")
            .Produces<GetAssetUnitByAssetIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Asset Units By Asset Id");
    }
}
