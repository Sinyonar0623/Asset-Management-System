using Shared.Security;

namespace Shared.Security.Authorization;

public static class RolePermissionMatrix
{
    private static readonly IReadOnlyDictionary<string, HashSet<string>> PermissionsByRole =
        new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
        {
            [RoleCodes.Admin] = [.. PermissionCatalog.All],
            [RoleCodes.Hod] =
            [
                PermissionCatalog.AssetRead,
                PermissionCatalog.RequestRead,
                PermissionCatalog.RequestWrite
            ],
            [RoleCodes.Teacher] =
            [
                PermissionCatalog.AssetRead,
                PermissionCatalog.RequestRead,
                PermissionCatalog.RequestWrite
            ],
            [RoleCodes.Student] =
            [
                PermissionCatalog.RequestRead,
                PermissionCatalog.RequestWrite
            ]
        };

    public static bool HasPermission(string roleCode, string permission)
    {
        if (string.IsNullOrWhiteSpace(roleCode) || string.IsNullOrWhiteSpace(permission))
        {
            return false;
        }

        return PermissionsByRole.TryGetValue(roleCode, out var permissions)
               && permissions.Contains(permission);
    }
}
