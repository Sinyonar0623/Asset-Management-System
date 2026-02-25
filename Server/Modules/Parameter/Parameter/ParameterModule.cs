using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parameter.Data;
using Parameter.Data.Repository;
using Parameter.Service;
using Shared.Data.Extensions;

namespace Parameter;

public static class ParameterModule
{
    public static IServiceCollection AddParameterModule(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddScoped<IParameterRepository, ParameterRepository>();
        service.AddScoped<IParameterService, ParameterService>();

        service.AddDbContext<ParameterDbContext>((sp, options) =>
        {
            var saveChangesInterceptors = sp.GetServices<ISaveChangesInterceptor>();
            options.AddInterceptors(saveChangesInterceptors);
            options.UseSqlServer(configuration.GetConnectionString("Database"), sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(ParameterDbContext).Assembly.GetName().Name);
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "parameter");
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
