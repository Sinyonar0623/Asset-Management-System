using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Messaging.Integration.Events;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMassTransitWithAssemblies(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
    {
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();
            config.AddConsumers(assemblies);

            config.UsingRabbitMq((context, c) =>
            {
                c.Host(new Uri(configuration["RabbitMQ:host"]!), host =>
               {
                   host.Username(configuration["RabbitMQ:Username"]!);
                   host.Password(configuration["RabbitMQ:Password"]!);

                   c.ConfigureEndpoints(context);
               });
            });
        });

        return services;
    }
}