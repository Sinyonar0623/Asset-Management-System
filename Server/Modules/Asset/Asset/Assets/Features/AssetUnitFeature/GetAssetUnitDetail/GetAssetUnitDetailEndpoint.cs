using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.AssetUnitFeature.GetAssetUnitDetail;

public class GetAssetUnitDetailEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/AssetUnit/{id:guid}/detail", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAssetUnitDetailQuery(id), cancellationToken);
                var response = new GetAssetUnitDetailResponse(result.AssetUnit);

                return Results.Ok(response);
            })
            .WithName("GetAssetUnitDetail")
            .Produces<GetAssetUnitDetailResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Asset Unit Detail");
    }
}
