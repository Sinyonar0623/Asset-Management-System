using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.CreateAsset;

public sealed class CreateAssetEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/assets", async (CreateAssetRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await sender.Send(
                        new CreateAssetCommand(
                            request.RealWorldId,
                            request.Brand,
                            request.Name,
                            request.SerialNo,
                            request.Description,
                            request.Type,
                            request.Amount,
                            request.Remark,
                            request.OwnerId),
                        cancellationToken);

                    return Results.Created($"/assets/{result.Id}", new CreateAssetResponse(result.Id));
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
            })
            .WithName("CreateAsset")
            .WithTags("Assets")
            .RequireAuthorization();
    }
}
