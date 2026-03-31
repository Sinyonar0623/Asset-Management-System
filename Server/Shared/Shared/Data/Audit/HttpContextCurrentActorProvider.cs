using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Shared.Data.Audit;

public class HttpContextCurrentActorProvider(IHttpContextAccessor httpContextAccessor) : ICurrentActorProvider
{
    private const string DefaultActor = "SYSTEM";
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string GetCurrentActor()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return DefaultActor;
        }

        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirst("sub")?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return DefaultActor;
        }

        return userId.ToString();
    }
}
