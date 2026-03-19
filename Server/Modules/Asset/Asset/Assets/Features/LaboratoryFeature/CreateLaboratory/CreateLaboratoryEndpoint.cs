using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.LaboratoryFeature.CreateLaboratory;

public class CreateLaboratoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/Laboratory",
            async (CreateLaboratoryRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new CreateLaboratoryCommand(request.Laboratory), cancellationToken);
                var response = new CreateLaboratoryResponse(result.Id);

                return Results.Ok(response);
            })
            .WithName("CreateLaboratory")
            .Produces<CreateLaboratoryResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Create Laboratory");
    }
}
