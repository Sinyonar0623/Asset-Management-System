using System.Security.Claims;

namespace Request.Requests.Features.Request.UpdateRequest;

public class UpdateRequestEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/Request/{id:guid}", async (
                Guid id,
                UpdateRequestRequest request,
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

                UpdateRequestResult result;
                try
                {
                    result = await sender.Send(new UpdateRequestCommand(id, request.Request, userId, roleCode), cancellationToken);
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Forbid();
                }
                catch (KeyNotFoundException)
                {
                    return Results.NotFound();
                }

                var response = new UpdateRequestResponse(result.IsSuccess);

                return Results.Ok(response);
            })
            .WithName("UpdateRequest")
            .Produces<UpdateRequestResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Request")
            .RequireAuthorization();
    }
}
