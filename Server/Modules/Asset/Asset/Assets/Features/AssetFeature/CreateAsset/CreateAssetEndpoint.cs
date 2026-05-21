using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.AssetFeature.CreateAsset;

public class CreateAssetEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/Asset", async (CreateAssetRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = request.Adapt<CreateAssetCommand>();

            var result = await sender.Send(command, cancellationToken);

            var response = result.Adapt<CreateAssetResponse>();

            return Results.Created($"/Asset/{response.Id}", response);
        }).WithName("CreateAsset")
        .Produces<CreateAssetResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Create Asset");
    }
}
