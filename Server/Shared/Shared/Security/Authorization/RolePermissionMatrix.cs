namespace Shared.Security.Authorization;

public static class RolePermissionMatrix
{
    private static class RoleCodes
    {
        public const string Admin = "00";
        public const string DepartmentHead = "01";
        public const string Lecturer = "02";
        public const string Student = "03";
    }

    private static readonly IReadOnlyDictionary<string, HashSet<string>> PermissionsByRole =
        new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
        {
            [RoleCodes.Admin] = [.. PermissionCatalog.All],
            [RoleCodes.DepartmentHead] =
            [
                PermissionCatalog.AssetRead,
                PermissionCatalog.RequestRead,
                PermissionCatalog.RequestWrite
            ],
            [RoleCodes.Lecturer] =
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
