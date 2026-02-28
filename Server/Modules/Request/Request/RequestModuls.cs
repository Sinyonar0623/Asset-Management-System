

namespace Request;

public static class RequestModule
{
    public static IServiceCollection AddRequestModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RequestDbContext>((sp, options) =>
        {
           var saveChangesInterceptors = sp.GetServices<ISaveChangesInterceptor>();
           options.AddInterceptors(saveChangesInterceptors);
           options.UseSqlServer(configuration.GetConnectionString("Database"), SqlOptions =>
           {
              SqlOptions.MigrationsAssembly(typeof(RequestDbContext).Assembly.GetName().Name);
              SqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "request");
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