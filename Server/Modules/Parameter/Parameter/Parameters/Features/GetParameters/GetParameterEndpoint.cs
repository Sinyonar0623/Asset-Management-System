using Microsoft.AspNetCore.Mvc;

namespace Parameter.Parameters.Features.GetParameters;

public class GetParameterEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Parameters",
            async ([AsParameters] PaginationRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(new GetParameterQuery(request), cancellationToken);

                return Results.Ok(response.Result);
            })
            .WithName("GetRequest")
            .Produces<GetParameterResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get all Parameter");
    }
}
