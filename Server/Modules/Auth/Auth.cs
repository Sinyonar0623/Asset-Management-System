using Shared.Data.Extensions;

namespace Auth;

public static class AuthModule
{
    public static IServiceCollection AddAuthModule(this IServiceCollection service, IConfiguration configuration)
    {
        
        return service;
    }

    public static IApplicationBuilder UseAuthModule(this IApplicationBuilder app)
    {
        // app.UseMigration();
        return app;
    }
}
