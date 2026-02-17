using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Auth.Authentication.Features.CreateUser;

public sealed class CreateUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/signIn/user", async (CreateUserRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new CreateUserCommand(request.Username, request.Email, request.Password, request.RoleCode),
                    cancellationToken);

                if (!result.Succeeded || result.UserId is null)
                {
                    return Results.BadRequest(new { message = result.Error });
                }

                return Results.Created($"/auth/users/{result.UserId}", new CreateUserResponse(result.UserId.Value));
            })
            .WithName("CreateUser")
            .WithTags("Auth");
    }
}
