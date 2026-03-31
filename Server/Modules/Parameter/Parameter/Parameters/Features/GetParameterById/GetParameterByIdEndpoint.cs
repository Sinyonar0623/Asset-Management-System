namespace Parameter.Parameters.Features.GetParameterById;

public class GetParameterByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Parameter/{id:long}", async(long id, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetParameterByIdQuery(id);

            var result = await sender.Send(query, cancellationToken);

            var response = result.Adapt<GetParameterByIdResponse>();

            return Results.Ok(response);

        }).WithName("GetRequestById")
        .Produces<GetParameterByIdResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get Parameter By Id");
    }
}