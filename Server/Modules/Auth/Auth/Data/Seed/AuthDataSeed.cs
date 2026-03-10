using System.Security.Cryptography.X509Certificates;
using Auth.Authentication.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Data;

namespace Auth.Data.Seed;

public class AuthDataSeed(AuthDbContext context) : IDataSeeder<AuthDbContext>
{
    public async Task SeedAllAsync()
    {
        if (!await context.UserRole.AnyAsync())
        {
            await context.UserRole.AddRangeAsync(InitialData.UserRoles);
            await context.SaveChangesAsync();
        }
    }
}