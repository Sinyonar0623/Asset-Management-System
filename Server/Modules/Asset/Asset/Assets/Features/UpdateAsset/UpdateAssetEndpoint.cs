using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.UpdateAsset;

public sealed class UpdateAssetEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/assets/{id:long}", async (long id, UpdateAssetRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new UpdateAssetCommand(id, request.Brand, request.Name, request.Description,
                        request.Type, request.Status, request.Amount, request.Remark),
                    cancellationToken);

                if (!result.Succeeded)
                    return Results.NotFound(new { message = result.Error });

                return Results.NoContent();
            })
            .WithName("UpdateAsset")
            .WithTags("Assets")
            .RequireAuthorization();
    }
}
