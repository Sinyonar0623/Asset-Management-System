using Auth.Authentication.Model;

namespace Auth.Data.Seed;

public static class InitialData
{
    public static List<UserRole> UserRoles { get; } =
    [
        CreateRole(Guid.Parse("1b4dc80d-c3e8-4e6d-a9d6-2bbd478d2d00"), "00", "ADMIN", "System administrator"),
        CreateRole(Guid.Parse("b89541a9-8ce6-4e95-820d-7c7785f96f01"), "01", "DEPTHEAD", "Department head"),
        CreateRole(Guid.Parse("52400f2b-0eaf-4921-9ad7-e527fb52bb02"), "02", "LECTURER", "Lecturer"),
        CreateRole(Guid.Parse("f801e95d-340b-4f34-8a6a-9deac4168003"), "03", "STUDENT", "Student")
    ];

    private static UserRole CreateRole(Guid id, string roleCode, string roleName, string roleDescription)
    {
        var role = UserRole.Create(roleCode, roleName, roleDescription);
        role.Id = id;
        role.CreateBy = "SYSTEM";
        return role;
    }
}
