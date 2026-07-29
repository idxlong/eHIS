using eHIS.Domain.Events;
using eHIS.Domain.SeedWork;
using eHIS.Domain.ValueObjects;

namespace eHIS.Domain.Aggregates.EncounterAggregate;

public class Encounter : AggregateRoot<string>
{
    public string Status { get; private set; } = null!; // planned | in-progress | onhold | discharged | completed | cancelled
    public Coding Class { get; private set; } = null!; // AMB | IMP | EMER etc.
    public string PatientId { get; private set; } = null!;
    public string PractitionerId { get; private set; } = null!;
    public Period Period { get; private set; } = null!;

    private readonly List<EncounterDiagnosis> _diagnoses = new();
    public IReadOnlyCollection<EncounterDiagnosis> Diagnoses => _diagnoses.AsReadOnly();

    private Encounter() { } // For EF Core

    public Encounter(
        string? id,
        string patientId,
        string practitionerId,
        Coding @class,
        DateTime startTime)
    {
        Id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;
        PatientId = patientId ?? throw new ArgumentNullException(nameof(patientId));
        PractitionerId = practitionerId ?? throw new ArgumentNullException(nameof(practitionerId));
        Class = @class ?? throw new ArgumentNullException(nameof(@class));
        Status = "in-progress";
        Period = Period.CreateOpen(startTime);

        AddDomainEvent(new EncounterStartedDomainEvent(Id, PatientId, PractitionerId));
    }

    public void AddDiagnosis(CodeableConcept condition, Coding? use = null, int? rank = null)
    {
        if (Status == "completed" || Status == "cancelled")
        {
            throw new InvalidOperationException("Cannot add diagnosis to a completed or cancelled encounter.");
        }

        var diagnosis = new EncounterDiagnosis(condition, use, rank);
        _diagnoses.Add(diagnosis);

        AddDomainEvent(new EncounterDiagnosisAddedDomainEvent(Id, condition));
    }

    public void CompleteEncounter(DateTime endTime)
    {
        if (Status != "in-progress" && Status != "onhold")
        {
            throw new InvalidOperationException($"Cannot complete encounter in status: {Status}");
        }

        Status = "completed";
        Period = Period.CreateClosed(Period.Start ?? DateTime.UtcNow, endTime);

        AddDomainEvent(new EncounterCompletedDomainEvent(Id, endTime));
    }

    public void CancelEncounter()
    {
        if (Status == "completed")
        {
            throw new InvalidOperationException("Cannot cancel a completed encounter.");
        }
        Status = "cancelled";
    }
}
