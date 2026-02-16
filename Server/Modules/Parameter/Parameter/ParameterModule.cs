using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parameter.Data;
using Shared.Data.Extensions;

namespace Parameter;

public static class ParameterModule
{
    public static IServiceCollection AddParameterModule(this IServiceCollection service, IConfiguration configuration)
    {
        return service;
    }

    public static IApplicationBuilder UseParameterModule(this IApplicationBuilder app)
    {
        app.UseMigration<ParameterDbContext>();
        return app;
    }
}