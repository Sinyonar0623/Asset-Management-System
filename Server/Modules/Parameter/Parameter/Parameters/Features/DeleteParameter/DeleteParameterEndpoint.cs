namespace Parameter.Parameters.Features.DeleteParameter;

public class DeleteParameterEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/Parameter/{id:long}",
            async (long id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteParameterCommand(id), cancellationToken);

                var response = result.Adapt<DeleteParameterResponse>();

                return Results.Ok(response);
            })
            .WithName("DeleteParameter")
            .Produces<DeleteParameterResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Parameter");
    }
}
