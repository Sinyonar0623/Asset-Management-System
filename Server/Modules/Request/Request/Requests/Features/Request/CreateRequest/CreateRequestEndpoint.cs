namespace Request.Requests.Features.Request.CreateRequest;

public class CreateRequestEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/Request", async (CreateRequestRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new CreateRequestCommand(request.Request), cancellationToken);
                var response = new CreateRequestResponse(result.Id);

                return Results.Created($"/Request/{response.Id}", response);
            })
            .WithName("CreateRequest")
            .Produces<CreateRequestResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Create Request");
    }
}
