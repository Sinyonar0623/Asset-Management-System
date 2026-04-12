

using Request.Service.EventHandlerService;

namespace Request;

public static class RequestModule
{
    public static IServiceCollection AddRequestModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRequestReadRepository, RequestReadRepository>();
        services.AddScoped<IRequestWriteRepository, RequestWriteRepository>();
        services.AddScoped<IRequestCommandHandlerService, RequestCommandHandlerService>();
        services.AddScoped<IRequestEventHandlerService, RequestEventHandlerService>();

        services.AddScoped<IUnitOfWork<RequestDbContext>, UnitOfWork<RequestDbContext>>();

        services.AddDbContext<RequestDbContext>((sp, options) =>
        {
           var saveChangesInterceptors = sp.GetServices<ISaveChangesInterceptor>();
           options.AddInterceptors(saveChangesInterceptors);
           options.UseNpgsql(configuration.GetConnectionString("Database"), npgsqlOptions =>
           {
              npgsqlOptions.MigrationsAssembly(typeof(RequestDbContext).Assembly.GetName().Name);
              npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "request");
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
