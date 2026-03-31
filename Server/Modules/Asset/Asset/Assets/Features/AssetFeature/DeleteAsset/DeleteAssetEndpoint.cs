using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.AssetFeature.DeleteAsset;

public class DeleteAssetEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/Asset/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteAssetCommand(id), cancellationToken);
                var response = new DeleteAssetResponse(result.IsSuccess);

                return Results.Ok(response);
            })
            .WithName("DeleteAsset")
            .Produces<DeleteAssetResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Asset");
    }
}
