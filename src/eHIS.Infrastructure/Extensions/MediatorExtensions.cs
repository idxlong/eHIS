using MediatR;
using Microsoft.EntityFrameworkCore;
using eHIS.Domain.SeedWork;

namespace eHIS.Infrastructure.Extensions;

public static class MediatorExtensions
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext ctx)
    {
        var domainEntities = ctx.ChangeTracker
            .Entries()
            .Where(x => x.Entity is AggregateRoot<string> root && root.DomainEvents.Count != 0)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => ((AggregateRoot<string>)x.Entity).DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => ((AggregateRoot<string>)entity.Entity).ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}
