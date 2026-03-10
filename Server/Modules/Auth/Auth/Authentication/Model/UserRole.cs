using Shared.DDD;

namespace Auth.Authentication.Model;

public class UserRole : Entity<Guid>
{
    public string RoleCode { get; private set; } = default!;
    public string RoleName { get; private set; } = default!;
    public string RoleDescription { get; private set; } = default!;

    private UserRole() { }

    private UserRole(string roleCode, string roleName, string roleDescription)
    {
        RoleCode = roleCode;
        RoleName = roleName;
        RoleDescription = roleDescription;
    }

    public static UserRole Create(string roleCode, string roleName, string roleDescription)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(roleCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(roleName);
        ArgumentException.ThrowIfNullOrWhiteSpace(roleDescription);

        return new UserRole(roleCode, roleName, roleDescription);
    }
}
