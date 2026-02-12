using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.DDD;

namespace Shared.Data.Interceptors;

public class DispatchDomainEventInterceptors(IMediator mediator) : SaveChangesInterceptor
{
    private async Task DispatchDomainEvent(DbContext? context)
    {
        if (context is null) return ;

        var aggregate = context.ChangeTracker
            .Entries<IAggregate>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);
        
        var domainEvents = aggregate
            .SelectMany(e => e.DomainEvents)
            .ToList();

        aggregate.ToList().ForEach(e => e.ClearDomainEvents());

        foreach(var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}