using Asset.Data;
using Asset.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data.Extensions;
using Shared.Data.UnitOfWork;

namespace Asset;

public static class AssetModule
{
    public static IServiceCollection AddAssetModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ILaboratoryRepository, LaboratoryRepository>();
        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IAssetUnitRepository, AssetUnitRepository>();
        services.AddScoped<IUnitOfWork<AssetDbContext>, UnitOfWork<AssetDbContext>>();

        services.AddDbContext<AssetDbContext>((sp, options) =>
        {
            var saveChangesInterceptors = sp.GetServices<ISaveChangesInterceptor>();
            options.AddInterceptors(saveChangesInterceptors);
            options.UseSqlServer(configuration.GetConnectionString("Database"), sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(AssetDbContext).Assembly.GetName().Name);
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "asset");
            });
        });

        return services;
    }

    public static IApplicationBuilder UseAssetModule(this IApplicationBuilder app)
    {
        app.UseMigration<AssetDbContext>();
        return app;
    }
}
