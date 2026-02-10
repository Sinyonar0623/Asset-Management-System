using Auth.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data.Extensions;

namespace Auth;

public static class AuthModule
{
    public static IServiceCollection AddAuthModule(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddDbContext<AuthDbContext>((sp, options) =>
        {
            var saveChangesInterceptor = sp.GetService<ISaveChangesInterceptor>();
            if (saveChangesInterceptor is not null) options.AddInterceptors(saveChangesInterceptor);
            options.UseSqlServer(configuration.GetConnectionString("Database"), sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(AuthDbContext).Assembly.GetName().Name);
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "auth");
            });
        });

        return service;
    }
    
    public static IApplicationBuilder UseAuthModule(this IApplicationBuilder app)
    {
        app.UseMigration<AuthDbContext>();

        return app;
    }
}