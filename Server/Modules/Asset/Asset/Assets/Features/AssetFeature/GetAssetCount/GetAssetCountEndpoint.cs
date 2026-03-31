using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.AssetFeature.GetAssetCount;

public class GetAssetCountEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Asset/count", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAssetCountQuery(), cancellationToken);
                var response = new GetAssetCountResponse(result.Count);

                return Results.Ok(response);
            })
            .WithName("GetAssetCount")
            .Produces<GetAssetCountResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Asset Count");
    }
}
