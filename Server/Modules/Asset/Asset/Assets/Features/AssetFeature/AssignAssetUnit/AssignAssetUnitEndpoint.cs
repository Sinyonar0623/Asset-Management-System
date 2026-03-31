using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.AssetFeature.AssignAssetUnit;

public class AssignAssetUnitEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/AssetAssignUnit", async (AssignAssetUnitRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = request.Adapt<AssignAssetUnitCommand>();

            var result = await sender.Send(command, cancellationToken);

            var response = result.Adapt<AssignAssetUnitResponse>();

            return Results.Ok(response);
        }).WithName("AssignAsset")
        .Produces<AssignAssetUnitResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Assign Asset");
    }
}