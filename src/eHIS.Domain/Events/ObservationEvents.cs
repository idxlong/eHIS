using eHIS.Domain.SeedWork;
using eHIS.Domain.ValueObjects;

namespace eHIS.Domain.Events;

public record ObservationRecordedDomainEvent(string ObservationId, string PatientId, CodeableConcept Code, Quantity Value) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
