using System.Security.Claims;

namespace Request.Requests.Features.Request.MarkProcessed;

public sealed class MarkProcessedEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/MarkProcessed", async (MarkProcessedRequest request, HttpContext httpContext, ISender sender, CancellationToken cancellationToken) =>
        {
            var userIdClaim = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? httpContext.User.FindFirstValue("sub");

            if (!Guid.TryParse(userIdClaim, out var approverId))
                return Results.Unauthorized();

            var roleCode = httpContext.User.FindFirstValue("role_code");

            if (string.IsNullOrWhiteSpace(roleCode))
                return Results.Unauthorized();

            var command = new MarkProcessedCommand(
                request.RequestId,
                approverId,
                roleCode,
                request.Decision,
                request.Comment,
                request.AssetIds ?? []);

            MarkProcessedResult result;
            try
            {
                result = await sender.Send(command, cancellationToken);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Forbid();
            }

            var response = new MarkProcessedResponse(result.IsSuccess);

            return Results.Ok(response);
        })
        .WithName("MarkProcessed")
        .Produces<MarkProcessedResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Approve or reject current approval step")
        .RequireAuthorization();
    }
}
