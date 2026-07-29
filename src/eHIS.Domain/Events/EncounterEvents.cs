using eHIS.Domain.SeedWork;
using eHIS.Domain.ValueObjects;

namespace eHIS.Domain.Events;

public record EncounterStartedDomainEvent(string EncounterId, string PatientId, string PractitionerId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record EncounterDiagnosisAddedDomainEvent(string EncounterId, CodeableConcept Diagnosis) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record EncounterCompletedDomainEvent(string EncounterId, DateTime EndTime) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
