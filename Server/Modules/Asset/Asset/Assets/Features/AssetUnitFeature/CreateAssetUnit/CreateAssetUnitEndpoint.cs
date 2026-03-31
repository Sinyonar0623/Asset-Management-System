using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.AssetUnitFeature.CreateAssetUnit;

public class CreateAssetUnitEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/AssetUnit", async (CreateAssetUnitRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateAssetUnitCommand(request.AssetUnits);

            var result = await sender.Send(command, cancellationToken);

            var response = new CreateAssetUnitResponse(result.Id);

            return Results.Ok(response);
        }).WithName("CreateAssetUnit")
        .Produces<CreateAssetUnitResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Create Asset Units");
    }
}
