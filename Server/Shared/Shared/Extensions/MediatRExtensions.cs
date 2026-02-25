using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Extensions;

public static class MediatRExtensions
{
    public static IServiceCollection AddMediatRWithAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    {
        
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssemblies(assemblies);
        });

        return services;
    }
}
