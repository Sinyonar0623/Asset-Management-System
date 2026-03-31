using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.LaboratoryFeature.GetLaboratory;

public class GetLaboratoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Laboratory", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetLaboratoryQuery(), cancellationToken);
                var response = new GetLaboratoryResponse(result.Laboratories);

                return Results.Ok(response);
            })
            .WithName("GetLaboratory")
            .Produces<GetLaboratoryResponse>()
            .WithSummary("Get Laboratory");
    }
}
