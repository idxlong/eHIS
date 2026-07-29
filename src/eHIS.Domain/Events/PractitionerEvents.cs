using eHIS.Domain.SeedWork;

namespace eHIS.Domain.Events;

public record PractitionerCreatedDomainEvent(string PractitionerId, string? Name) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
