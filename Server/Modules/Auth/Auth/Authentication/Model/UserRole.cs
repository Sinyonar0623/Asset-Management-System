using Shared.DDD;

namespace Auth.Authentication.Model;

public class UserRole : Entity<Guid>
{
    public string RoleCode { get; private set; } = default!;
    public string RoleName { get; private set; } = default!;
    public string RoleDescription { get; private set; } = default!;
}