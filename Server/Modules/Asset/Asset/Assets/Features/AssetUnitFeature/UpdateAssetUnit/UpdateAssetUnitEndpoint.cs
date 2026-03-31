using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.AssetUnitFeature.UpdateAssetUnit;

public class UpdateAssetUnitEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/AssetUnit/{id:guid}",
            async (Guid id, UpdateAssetUnitRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new UpdateAssetUnitCommand(id, request.AssetUnit),
                    cancellationToken);

                var response = new UpdateAssetUnitResponse(result.IsSuccess);

                return Results.Ok(response);
            })
            .WithName("UpdateAssetUnit")
            .Produces<UpdateAssetUnitResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Asset Unit");
    }
}
