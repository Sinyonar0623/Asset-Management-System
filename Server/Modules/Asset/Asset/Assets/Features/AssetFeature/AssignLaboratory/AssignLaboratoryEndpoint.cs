using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.AssetFeature.AssignLaboratory;

public class AssignLaboratoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/AssetAssignLaboratory", async (AssignLaboratoryRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = request.Adapt<AssignLaboratoryCommand>();
            var result = await sender.Send(command, cancellationToken);
            var response = result.Adapt<AssignLaboratoryResponse>();

            return Results.Ok(response);
        }).WithName("AssignLaboratory")
        .Produces<AssignLaboratoryResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Assign Laboratory");
    }
}
