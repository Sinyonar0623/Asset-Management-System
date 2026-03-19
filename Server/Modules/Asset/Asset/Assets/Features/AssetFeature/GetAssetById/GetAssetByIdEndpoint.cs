using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.AssetFeature.GetAssetById;

public class GetAssetByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Asset/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAssetByIdQuery(id), cancellationToken);
                var response = new GetAssetByIdResponse(result.Asset);

                return Results.Ok(response);
            })
            .WithName("GetAssetById")
            .Produces<GetAssetByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Asset By Id");
    }
}
