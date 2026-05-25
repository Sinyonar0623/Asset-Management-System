namespace Request.Requests.Features.Request.DeleteRequest;

public class DeleteRequestEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/Request/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteRequestCommand(id), cancellationToken);
                var response = new DeleteRequestResponse(result.IsSuccess);

                return Results.Ok(response);
            })
            .WithName("DeleteRequest")
            .Produces<DeleteRequestResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Request");
    }
}
