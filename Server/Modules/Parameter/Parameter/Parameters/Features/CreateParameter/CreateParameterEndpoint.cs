namespace Parameter.Parameters.Features.CreateParameter;

public class CreateParameterEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/Parameter", async (ParameterDto parameter, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateParameterCommand(parameter);

            var result = await sender.Send(command, cancellationToken);

            var response = result.Adapt<CreateParameterResponse>();

            return Results.Created();
        }).WithName("CreateParameter")
        .Produces<CreateParameterResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Create Parameter");
    }
}
