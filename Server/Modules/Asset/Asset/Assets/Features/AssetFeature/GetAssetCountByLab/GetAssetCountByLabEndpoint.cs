using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.AssetFeature.GetAssetCountByLab;

public class GetAssetCountByLabEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Asset/laboratory/{laboratoryId:guid}/count",
            async (Guid laboratoryId, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAssetCountByLabQuery(laboratoryId), cancellationToken);
                var response = new GetAssetCountByLabResponse(result.Count);

                return Results.Ok(response);
            })
            .WithName("GetAssetCountByLab")
            .Produces<GetAssetCountByLabResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Asset Count By Laboratory");
    }
}
