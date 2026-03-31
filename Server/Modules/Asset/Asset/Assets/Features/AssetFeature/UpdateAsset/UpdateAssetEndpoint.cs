using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.AssetFeature.UpdateAsset;

public class UpdateAssetEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/Asset/{id:guid}",
            async (Guid id, UpdateAssetRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new UpdateAssetCommand(id, request.Asset), cancellationToken);
                var response = new UpdateAssetResponse(result.IsSuccess);

                return Results.Ok(response);
            })
            .WithName("UpdateAsset")
            .Produces<UpdateAssetResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Asset");
    }
}
