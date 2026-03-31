using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parameter.Data;
using Parameter.Data.Repository;
using Shared.Data.Extensions;
using Shared.Data.UnitOfWork;

namespace Parameter;

public static class ParameterModule
{
    public static IServiceCollection AddParameterModule(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddScoped<IParameterService, ParameterService>();

        service.AddScoped<IParameterRepository, ParameterRepository>();
        service.AddScoped<IUnitOfWork<ParameterDbContext>, UnitOfWork<ParameterDbContext>>();

        service.AddDbContext<ParameterDbContext>((sp, options) =>
        {
            var saveChangesInterceptors = sp.GetServices<ISaveChangesInterceptor>();
            options.AddInterceptors(saveChangesInterceptors);
            options.UseNpgsql(configuration.GetConnectionString("Database"), npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(ParameterDbContext).Assembly.GetName().Name);
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "parameter");
            });
        });

        return service;
    }

    public static IApplicationBuilder UseParameterModule(this IApplicationBuilder app)
    {
        app.UseMigration<ParameterDbContext>();
        return app;
    }
}
