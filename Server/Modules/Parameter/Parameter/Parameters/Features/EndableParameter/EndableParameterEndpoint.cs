namespace Parameter.Parameters.Features.EndableParameter;

public class EndableParameterEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/Parameter/Endable/{id:long}",
            async (long id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new EndableParameterCommand(id), cancellationToken);

                var response = result.Adapt<EndableParameterResponse>();

                return Results.Ok(response);
            })
            .WithName("EndableParameter")
            .Produces<EndableParameterResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Endable Parameter");
    }
}
