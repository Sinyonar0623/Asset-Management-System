using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.DeleteAsset;

public sealed class DeleteAssetEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/assets/{id:long}", async (long id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteAssetCommand(id), cancellationToken);

                if (!result.Succeeded)
                    return Results.NotFound(new { message = result.Error });

                return Results.NoContent();
            })
            .WithName("DeleteAsset")
            .WithTags("Assets")
            .RequireAuthorization();
    }
}
