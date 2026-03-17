using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.AssetUnitFeature.DeleteAssetUnit;

public class DeleteAssetUnitEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/AssetUnit/{id:guid}",
            async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteAssetUnitCommand(id), cancellationToken);
                var response = new DeleteAssetUnitResponse(result.Id);

                return Results.Ok(response);
            })
            .WithName("DeleteAssetUnit")
            .Produces<DeleteAssetUnitResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Asset Unit");
    }
}
