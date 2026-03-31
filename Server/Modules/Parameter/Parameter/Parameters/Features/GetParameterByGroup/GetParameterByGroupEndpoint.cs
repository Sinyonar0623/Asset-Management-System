using Microsoft.AspNetCore.Mvc;

namespace Parameter.Parameters.Features.GetParameterByGroup;

public class GetParameterByGroupEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Parameters/{group}", async (string group, [AsParameters] PaginationRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetParameterByGroupQuery(request, group);

            var response = await sender.Send(query, cancellationToken);

            return Results.Ok(response.Result);

        }).WithName("GetParameterByGroup")
        .Produces<GetParameterByGroupResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get all Parameter By Group");
    }
}
