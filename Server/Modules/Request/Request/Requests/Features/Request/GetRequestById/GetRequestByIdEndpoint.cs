namespace Request.Requests.Features.Request.GetRequestById;

public class GetRequestByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Request/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetRequestByIdQuery(id), cancellationToken);
                var response = new GetRequestByIdResponse(result.Request);

                return Results.Ok(response);
            })
            .WithName("GetRequestById")
            .Produces<GetRequestByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Request By Id");
    }
}
