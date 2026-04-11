namespace Request.Requests.Features.Request.UpdateRequest;

public class UpdateRequestEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/Request/{id:guid}", async (Guid id, UpdateRequestRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new UpdateRequestCommand(id, request.Request), cancellationToken);
                var response = new UpdateRequestResponse(result.IsSuccess);

                return Results.Ok(response);
            })
            .WithName("UpdateRequest")
            .Produces<UpdateRequestResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Request");
    }
}
