using Mapster;

namespace Request.Requests.Features.Request.MarkApproved;

public sealed class MarkApprovedEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/MarkApproved", async (MarkApprovedRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = request.Adapt<MarkApprovedCommand>();

            var result = await sender.Send(command, cancellationToken);

            var response = result.Adapt<MarkApprovedResponse>();

            return Results.Ok(response);
        })
        .WithName("MarkApproved")
        .Produces<MarkApprovedResponse>(StatusCodes.Status202Accepted)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Mark request as approved");
    }
}
