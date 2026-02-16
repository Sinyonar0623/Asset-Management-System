using Asset.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data.Extensions;

namespace Asset;

public static class AssetModule
{
    public static IServiceCollection AddAssetModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AssetDbContext>((sp, options) =>
        {
            var saveChangesInterceptor = sp.GetService<ISaveChangesInterceptor>();
            if (saveChangesInterceptor is not null) options.AddInterceptors(saveChangesInterceptor);
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
