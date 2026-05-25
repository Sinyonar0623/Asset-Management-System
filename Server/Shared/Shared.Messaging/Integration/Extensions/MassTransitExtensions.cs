using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Messaging.Integration.Extensions;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMassTransitWithAssemblies(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
    {
        var host = configuration["RabbitMQ:Host"];
        var username = configuration["RabbitMQ:Username"];
        var password = configuration["RabbitMQ:Password"];

        if (string.IsNullOrWhiteSpace(host)
            || string.IsNullOrWhiteSpace(username)
            || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("Missing RabbitMQ configuration. Configure RabbitMQ:Host, RabbitMQ:Username and RabbitMQ:Password.");
        }

        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();
            config.AddConsumers(assemblies);

            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(host), hostConfig =>
                {
                    hostConfig.Username(username);
                    hostConfig.Password(password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
