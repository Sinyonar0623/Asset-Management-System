namespace Parameter.Parameters.Features.UpdateParameter;

public class UpdateParameterEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/Parameter/{id:long}",
            async (long id, ParameterDto parameter, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new UpdateParameterCommand(id, parameter);

                var result = await sender.Send(command, cancellationToken);

                var response = result.Adapt<UpdateParameterResponse>();

                return Results.Ok(response);
            })
            .WithName("UpdateParameter")
            .Produces<UpdateParameterResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Parameter");
    }
}
