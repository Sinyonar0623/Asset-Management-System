using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.AssetUnitFeature.GetAssetUnitById;

public class GetAssetUnitByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/AssetUnit/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAssetUnitByIdQuery(id), cancellationToken);
                var response = new GetAssetUnitByIdResponse(result.AssetUnit);

                return Results.Ok(response);
            })
            .WithName("GetAssetUnitById")
            .Produces<GetAssetUnitByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Asset Unit By Id");
    }
}
