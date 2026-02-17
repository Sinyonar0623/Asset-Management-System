using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Auth.Authentication.Features.SignUp;

public sealed class SignUpEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/signup/user", async (SignUpRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new SignUpCommand(request.Username, request.Email, request.Password, request.RoleCode),
                    cancellationToken);

                if (!result.Succeeded || result.UserId is null)
                {
                    return Results.BadRequest(new { message = result.Error });
                }

                return Results.Created($"/auth/users/{result.UserId}", new SignUpResponse(result.UserId.Value));
            })
            .WithName("SignUp")
            .WithTags("Auth");
    }
}
