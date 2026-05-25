using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;
using System.Security.Claims;

namespace Request.Requests.Features.Request.GetRequest;

public class GetRequestEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Request", async (
                [AsParameters] PaginationRequest request,
                HttpContext httpContext,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? httpContext.User.FindFirstValue("sub");

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var roleCode = httpContext.User.FindFirstValue("role_code");
                if (string.IsNullOrWhiteSpace(roleCode))
                {
                    return Results.Unauthorized();
                }

                var result = await sender.Send(new GetRequestQuery(request, userId, roleCode), cancellationToken);
                var response = new GetRequestResponse(result.Requests);

                return Results.Ok(response);
            })
            .WithName("GetRequests")
            .Produces<GetRequestResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Request")
            .RequireAuthorization();
    }
}
