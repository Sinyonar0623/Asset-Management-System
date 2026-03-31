using Asset.Configuration;
using Asset.Data;
using Asset.Data.Repository.Read;
using Asset.Data.Repository.Write;
using Asset.Service;
using Asset.Service.CommandHandlerService;
using Asset.Service.EventHandlerService;
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
        MappingConfiguration.ConfigurationMappings();

        services.AddScoped<ILaboratoryReadRepository, LaboratoryReadRepository>();
        services.AddScoped<ILaboratoryWriteRepository, LaboratoryWriteRepository>();
        services.AddScoped<ILaboratoryCommandHandlerService, LaboratoryCommandHandlerService>();

        services.AddScoped<IAssetCommandHandlerService, AssetCommandHandlerService>();
        services.AddScoped<IAssetReadRepository, AssetReadRepository>();
        services.AddScoped<IAssetWriteRepository, AssetWriteRepository>();

        services.AddScoped<IAssetUnitReadRepository, AssetUnitReadRepository>();
        services.AddScoped<IAssetUnitWriteRepository, AssetUnitWriteRepository>();
        services.AddScoped<IAssetUnitCommandHandlerService, AssetUnitCommandHandlerService>();
        services.AddScoped<IAssetUnitEventHandlerService, AssetUnitEventHandlerService>();

        services.AddScoped<IUnitOfWork<AssetDbContext>, UnitOfWork<AssetDbContext>>();

        services.AddDbContext<AssetDbContext>((sp, options) =>
        {
            var saveChangesInterceptors = sp.GetServices<ISaveChangesInterceptor>();
            options.AddInterceptors(saveChangesInterceptors);
            options.UseNpgsql(configuration.GetConnectionString("Database"), npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(AssetDbContext).Assembly.GetName().Name);
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "asset");
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
