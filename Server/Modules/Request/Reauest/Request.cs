using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reauest.Data;
using Reauest.Data.Repository;
using Shared.Data.Extensions;

namespace Reauest;

public static class RequestModule
{
    public static IServiceCollection AddRequestModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRequestRepository, RequestRepository>();

        services.AddDbContext<RequestDbContext>((sp, options) =>
        {
            var saveChangesInterceptors = sp.GetServices<ISaveChangesInterceptor>();
            options.AddInterceptors(saveChangesInterceptors);
            options.UseSqlServer(configuration.GetConnectionString("Database"), sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(RequestDbContext).Assembly.GetName().Name);
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "request");
            });
        });

        return services;
    }

    public static IApplicationBuilder UseRequestModule(this IApplicationBuilder app)
    {
        app.UseMigration<RequestDbContext>();
        return app;
    }
}
