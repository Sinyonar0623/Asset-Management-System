using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.LaboratoryFeature.GetLaboratoryById;

public class GetLaboratoryByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Laboratory/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetLaboratoryByIdQuery(id), cancellationToken);
                var response = new GetLaboratoryByIdResponse(result.Laboratory);

                return Results.Ok(response);
            })
            .WithName("GetLaboratoryById")
            .Produces<GetLaboratoryByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Laboratory By Id");
    }
}
