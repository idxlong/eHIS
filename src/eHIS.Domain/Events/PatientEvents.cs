using eHIS.Domain.SeedWork;
using eHIS.Domain.ValueObjects;

namespace eHIS.Domain.Events;

public record PatientCreatedDomainEvent(string PatientId, string? Name) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record PatientDemographicsUpdatedDomainEvent(string PatientId, string? Name) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record PatientDeactivatedDomainEvent(string PatientId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
