namespace Parameter.Parameters.Features.DisableParameter;

public class DisableParameterEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/Parameter/Disable/{id:long}",
            async (long id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DisableParameterCommand(id), cancellationToken);

                var response = result.Adapt<DisableParameterResponse>();

                return Results.Ok(response);
            })
            .WithName("DisableParameter")
            .Produces<DisableParameterResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Disable Parameter");
    }
}
