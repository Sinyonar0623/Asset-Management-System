using Auth.Authentication.Model;
using Auth.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Data.Seeder;

public static class AuthDataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<UserName>>();

        await SeedRolesAsync(dbContext);
        await SeedUsersAsync(dbContext, passwordHasher);
    }

    private static async Task SeedRolesAsync(AuthDbContext dbContext)
    {
        var roles = new[]
        {
            ("00", "Admin", "ผู้ดูแลระบบ"),
            ("01", "Department Head", "หัวหน้าภาควิชา"),
            ("02", "Lecturer", "อาจารย์"),
            ("03", "Student", "นิสิต"),
        };

        foreach (var (code, name, desc) in roles)
        {
            var exists = await dbContext.Set<UserRole>().AnyAsync(r => r.RoleCode == code);
            if (!exists)
            {
                await dbContext.Database.ExecuteSqlRawAsync(
                    "INSERT INTO [auth].[UserRole] (RoleCode, RoleName, RoleDescription, CreateOn, CreateBy) VALUES ({0}, {1}, {2}, GETUTCDATE(), 'system')",
                    code, name, desc
                );
            }
        }
    }

    private static async Task SeedUsersAsync(AuthDbContext dbContext, IPasswordHasher<UserName> passwordHasher)
    {
        var seedUsers = new[]
        {
            ("admin",   "admin@ce.ku.ac.th",    "00"),
            ("depthead","depthead@ce.ku.ac.th",  "01"),
            ("lecturer","lecturer@ce.ku.ac.th",  "02"),
            ("student", "student@ce.ku.ac.th",   "03"),
        };

        foreach (var (username, email, roleCode) in seedUsers)
        {
            var exists = await dbContext.Auth.AnyAsync(u => u.Email == email);
            if (exists) continue;

            var role = await dbContext.Set<UserRole>().FirstOrDefaultAsync(r => r.RoleCode == roleCode);
            if (role is null) continue;

            var user = UserName.Create(username, email);
            user.SetPasswordHash(passwordHasher.HashPassword(user, "Admin@1234"));
            user.AssignRole(role);

            await dbContext.Auth.AddAsync(user);
            await dbContext.SaveChangesAsync();
        }
    }
}
