using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.GetAssets;

public sealed class GetAssetsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/assets", async (
                ISender sender,
                CancellationToken cancellationToken,
                int page = 1,
                int pageSize = 20,
                string? search = null,
                string? type = null,
                string? status = null) =>
            {
                var result = await sender.Send(
                    new GetAssetsQuery(page, pageSize, search, type, status),
                    cancellationToken);

                return Results.Ok(result.Data);
            })
            .WithName("GetAssets")
            .WithTags("Assets")
            .RequireAuthorization();
    }
}
