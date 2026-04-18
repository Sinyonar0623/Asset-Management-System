using System.Security.Claims;

namespace Request.Requests.Features.Request.CreateRequest;

public class CreateRequestEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/Request", async (CreateRequestRequest request, HttpContext httpContext, ISender sender, CancellationToken cancellationToken) =>
            {
                var userIdClaim = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? httpContext.User.FindFirstValue("sub");

                if (!Guid.TryParse(userIdClaim, out var requesterId))
                {
                    return Results.Unauthorized();
                }

                var result = await sender.Send(new CreateRequestCommand(request.Request, requesterId), cancellationToken);
                var response = new CreateRequestResponse(result.Id);

                return Results.Created($"/Request/{response.Id}", response);
            })
            .WithName("CreateRequest")
            .Produces<CreateRequestResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Create Request")
            .RequireAuthorization();
    }
}
