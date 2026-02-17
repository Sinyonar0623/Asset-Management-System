using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Auth.Authentication.Features.Login;

public sealed class LoginEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (LoginRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new LoginCommand(request.Email, request.Password),
                    cancellationToken);

                if (!result.Succeeded || result.Response is null)
                {
                    if (result.IsValidationError)
                    {
                        return Results.BadRequest(new { message = result.Error });
                    }

                    if (result.IsAlreadyLoggedIn)
                    {
                        return Results.Json(
                            new { message = result.Error ?? "User is already logged in." },
                            statusCode: StatusCodes.Status409Conflict);
                    }

                    return Results.Json(
                        new { message = result.Error ?? "Invalid email or password." },
                        statusCode: StatusCodes.Status401Unauthorized);
                }

                return Results.Ok(result.Response);
            })
            .WithName("Login")
            .WithTags("Auth");
    }
}
