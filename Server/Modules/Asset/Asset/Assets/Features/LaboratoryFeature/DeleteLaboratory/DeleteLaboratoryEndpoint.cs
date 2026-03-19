using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.LaboratoryFeature.DeleteLaboratory;

public class DeleteLaboratoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/Laboratory/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteLaboratoryCommand(id), cancellationToken);
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result.Message ?? "Cannot delete laboratory.");
                }

                var response = new DeleteLaboratoryResponse(result.IsSuccess);
                return Results.Ok(response);
            })
            .WithName("DeleteLaboratory")
            .Produces<DeleteLaboratoryResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete Laboratory");
    }
}
