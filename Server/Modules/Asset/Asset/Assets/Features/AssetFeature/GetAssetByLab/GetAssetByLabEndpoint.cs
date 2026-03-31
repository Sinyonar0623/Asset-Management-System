using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Pagination;

namespace Asset.Assets.Features.AssetFeature.GetAssetByLab;

public class GetAssetByLabEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Asset/laboratory/{laboratoryId:guid}",
            async (Guid laboratoryId, [AsParameters] PaginationRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAssetByLabQuery(laboratoryId, request), cancellationToken);
                var response = new GetAssetByLabResponse(result.Assets);

                return Results.Ok(response);
            })
            .WithName("GetAssetByLab")
            .Produces<GetAssetByLabResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Asset By Laboratory");
    }
}
