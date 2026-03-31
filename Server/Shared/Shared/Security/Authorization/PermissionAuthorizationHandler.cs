using Microsoft.AspNetCore.Authorization;

namespace Shared.Security.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var roleCode = context.User.FindFirst("role_code")?.Value;
        if (RolePermissionMatrix.HasPermission(roleCode ?? string.Empty, requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
