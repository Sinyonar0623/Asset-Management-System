using Microsoft.EntityFrameworkCore;
using Shared.Data;

namespace Parameter.Data.Seed;

public class ParameterDataSeed(ParameterDbContext context) : IDataSeeder<ParameterDbContext>
{
    public async Task SeedAllAsync()
    {
        var existingKeys = await context.Parameter
            .Select(x => new { x.Group, x.Value })
            .ToListAsync();

        var existingKeySet = existingKeys
            .Select(x => $"{x.Group}|{x.Value}")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missingParameters = InitialParameterData.Parameters
            .Where(x => !existingKeySet.Contains($"{x.Group}|{x.Value}"))
            .ToList();

        if (missingParameters.Count == 0)
        {
            return;
        }

        await context.Parameter.AddRangeAsync(missingParameters);
        await context.SaveChangesAsync();
    }
}
