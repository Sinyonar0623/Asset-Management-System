using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Parameter.Data.Repository;

namespace Parameter.Parameters.Features.GetParameters;

public sealed class GetParametersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/parameters", async (IParameterRepository repository, CancellationToken cancellationToken, string? group = null) =>
            {
                if (!string.IsNullOrWhiteSpace(group))
                {
                    var items = await repository.GetParameterByGroup(group);
                    return Results.Ok(items);
                }

                var all = await repository.GetAllAsync(cancellationToken);
                var result = all.Select(p => new
                {
                    p.Id,
                    p.Group,
                    p.Value,
                    p.Description,
                    p.Active
                });
                return Results.Ok(result);
            })
            .WithName("GetParameters")
            .WithTags("Parameters")
            .RequireAuthorization();

        app.MapPost("/parameters", async (CreateParameterRequest request, IParameterRepository repository, CancellationToken cancellationToken) =>
            {
                var parameter = Parameters.Model.Parameter.Create(request.Group, request.Value, request.Description);
                await repository.AddAsync(parameter, cancellationToken);
                await repository.SaveChangeAsync(cancellationToken);
                return Results.Created($"/parameters/{parameter.Id}", new { parameter.Id });
            })
            .WithName("CreateParameter")
            .WithTags("Parameters")
            .RequireAuthorization();

        app.MapPut("/parameters/{id:long}/disable", async (long id, IParameterRepository repository, CancellationToken cancellationToken) =>
            {
                var parameter = await repository.GetByIdAsync(id, cancellationToken);
                if (parameter is null)
                    return Results.NotFound(new { message = "Parameter not found." });

                parameter.Disable();
                await repository.SaveChangeAsync(cancellationToken);
                return Results.NoContent();
            })
            .WithName("DisableParameter")
            .WithTags("Parameters")
            .RequireAuthorization();

        app.MapPut("/parameters/{id:long}/enable", async (long id, IParameterRepository repository, CancellationToken cancellationToken) =>
            {
                var parameter = await repository.GetByIdAsync(id, cancellationToken);
                if (parameter is null)
                    return Results.NotFound(new { message = "Parameter not found." });

                parameter.Enable();
                await repository.SaveChangeAsync(cancellationToken);
                return Results.NoContent();
            })
            .WithName("EnableParameter")
            .WithTags("Parameters")
            .RequireAuthorization();

        app.MapDelete("/parameters/{id:long}", async (long id, IParameterRepository repository, CancellationToken cancellationToken) =>
            {
                var parameter = await repository.GetByIdAsync(id, cancellationToken);
                if (parameter is null)
                    return Results.NotFound(new { message = "Parameter not found." });

                await repository.DeleteAsync(parameter, cancellationToken);
                await repository.SaveChangeAsync(cancellationToken);
                return Results.NoContent();
            })
            .WithName("DeleteParameter")
            .WithTags("Parameters")
            .RequireAuthorization();
    }
}

public record CreateParameterRequest(string Group, string Value, string Description);
