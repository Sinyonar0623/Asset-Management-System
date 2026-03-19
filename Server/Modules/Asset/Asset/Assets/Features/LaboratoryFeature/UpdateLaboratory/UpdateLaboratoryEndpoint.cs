using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.LaboratoryFeature.UpdateLaboratory;

public class UpdateLaboratoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/Laboratory/{id:guid}",
            async (Guid id, UpdateLaboratoryRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new UpdateLaboratoryCommand(id, request.Laboratory), cancellationToken);
                
                var response = new UpdateLaboratoryResponse(result.IsSuccess);

                return Results.Ok(response);
            })
            .WithName("UpdateLaboratory")
            .Produces<UpdateLaboratoryResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Laboratory");
    }
}
