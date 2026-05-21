using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.AssetUnitFeature.GetUnassignedAssetUnits;

public class GetUnassignedAssetUnitsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/AssetUnit/unassigned", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetUnassignedAssetUnitsQuery(), cancellationToken);
                var response = new GetUnassignedAssetUnitsResponse(result.AssetUnits);

                return Results.Ok(response);
            })
            .WithName("GetUnassignedAssetUnits")
            .Produces<GetUnassignedAssetUnitsResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Unassigned Asset Units");
    }
}
