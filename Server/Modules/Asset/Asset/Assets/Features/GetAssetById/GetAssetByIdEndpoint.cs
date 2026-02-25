using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.GetAssetById;

public sealed class GetAssetByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/assets/{id:long}", async (long id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAssetByIdQuery(id), cancellationToken);

                if (!result.Found || result.Asset is null)
                    return Results.NotFound(new { message = "Asset not found." });

                return Results.Ok(result.Asset);
            })
            .WithName("GetAssetById")
            .WithTags("Assets")
            .RequireAuthorization();
    }
}
