using System.Security.Claims;

namespace Request.Requests.Features.Request.GetRequestById;

public class GetRequestByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Request/{id:guid}", async (
                Guid id,
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

                GetRequestByIdResult result;
                try
                {
                    result = await sender.Send(new GetRequestByIdQuery(id, userId, roleCode), cancellationToken);
                }
                catch (KeyNotFoundException)
                {
                    return Results.NotFound();
                }

                var response = new GetRequestByIdResponse(result.Request);

                return Results.Ok(response);
            })
            .WithName("GetRequestById")
            .Produces<GetRequestByIdResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Request By Id")
            .RequireAuthorization();
    }
}
