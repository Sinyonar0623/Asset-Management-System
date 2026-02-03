using Microsoft.EntityFrameworkCore;

namespace Shared.Data;

public interface IDataSeeder
{
    Task SeedAllAsync();
}

public interface IDataSeeder<TContext> : IDataSeeder where TContext : DbContext;