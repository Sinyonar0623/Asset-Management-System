using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;

namespace Request.Requests.Features.Request.GetRequest;

public class GetRequestEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Request", async ([AsParameters] PaginationRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetRequestQuery(request), cancellationToken);
                var response = new GetRequestResponse(result.Requests);

                return Results.Ok(response);
            })
            .WithName("GetRequests")
            .Produces<GetRequestResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Request");
    }
}
