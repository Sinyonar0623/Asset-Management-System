using Asset.Data.Repository;
using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.GetLaboratories;

public sealed class GetLaboratoriesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/assets/laboratories", async (IAssetRepository repository, CancellationToken cancellationToken) =>
            {
                var labs = await repository.GetAllLaboratoriesAsync(cancellationToken);
                var result = labs.Select(l => new
                {
                    l.Id,
                    l.LaboratoryName,
                    l.RoomNo,
                    l.TeacherId,
                    l.Description
                });
                return Results.Ok(result);
            })
            .WithName("GetLaboratories")
            .WithTags("Assets")
            .RequireAuthorization();

        app.MapPost("/assets/laboratories", async (CreateLaboratoryRequest request, IAssetRepository repository, CancellationToken cancellationToken) =>
            {
                var lab = Assets.Model.AssetLaboratory.Create(
                    request.LaboratoryName,
                    request.RoomNo,
                    request.TeacherId,
                    request.Description);

                await repository.AddLaboratoryAsync(lab, cancellationToken);
                await repository.SaveChangesAsync(cancellationToken);

                return Results.Created($"/assets/laboratories/{lab.Id}", new { lab.Id });
            })
            .WithName("CreateLaboratory")
            .WithTags("Assets")
            .RequireAuthorization();
    }
}

public record CreateLaboratoryRequest(
    string LaboratoryName,
    string RoomNo,
    Guid TeacherId,
    string Description
);
