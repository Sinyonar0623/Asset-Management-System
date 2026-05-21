using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Auth.Authentication.Features.GetUserLookup;

public sealed class GetUserLookupEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/users/lookup", async (
                string? ids,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var userIds = ParseUserIds(ids);
                if (userIds.Count == 0)
                {
                    return Results.Ok(new GetUserLookupResponse([]));
                }

                var result = await sender.Send(new GetUserLookupQuery(userIds), cancellationToken);
                var response = new GetUserLookupResponse(result.Users);

                return Results.Ok(response);
            })
            .WithName("GetUserLookup")
            .Produces<GetUserLookupResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Auth")
            .WithSummary("Get user display names by ids")
            .RequireAuthorization();
    }

    private static List<Guid> ParseUserIds(string? ids)
    {
        if (string.IsNullOrWhiteSpace(ids))
        {
            return [];
        }

        return ids
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(value => Guid.TryParse(value, out var userId) ? userId : Guid.Empty)
            .Where(userId => userId != Guid.Empty)
            .Distinct()
            .Take(100)
            .ToList();
    }
}
