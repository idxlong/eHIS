using eHIS.Domain.Events;
using eHIS.Domain.SeedWork;
using eHIS.Domain.ValueObjects;

namespace eHIS.Domain.Aggregates.ObservationAggregate;

public class Observation : AggregateRoot<string>
{
    public string Status { get; private set; } = null!; // registered | preliminary | final | amended | cancelled
    public CodeableConcept Code { get; private set; } = null!;
    public string PatientId { get; private set; } = null!;
    public string? EncounterId { get; private set; }
    public DateTime EffectiveDateTime { get; private set; }
    public Quantity Value { get; private set; } = null!;
    public string PerformerId { get; private set; } = null!;

    private readonly List<CodeableConcept> _categories = new();
    public IReadOnlyCollection<CodeableConcept> Category => _categories.AsReadOnly();

    private Observation() { } // For EF Core

    public Observation(
        string? id,
        string patientId,
        string performerId,
        CodeableConcept code,
        Quantity value,
        DateTime effectiveDateTime,
        string? encounterId = null,
        IEnumerable<CodeableConcept>? categories = null)
    {
        Id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;
        PatientId = patientId ?? throw new ArgumentNullException(nameof(patientId));
        PerformerId = performerId ?? throw new ArgumentNullException(nameof(performerId));
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Value = value ?? throw new ArgumentNullException(nameof(value));
        EffectiveDateTime = effectiveDateTime;
        EncounterId = encounterId;
        Status = "final"; // Default FHIR status for completed observations

        if (categories != null)
        {
            _categories.AddRange(categories);
        }

        AddDomainEvent(new ObservationRecordedDomainEvent(Id, PatientId, Code, Value));
    }

    public void UpdateValue(Quantity newValue, DateTime effectiveDateTime)
    {
        Value = newValue ?? throw new ArgumentNullException(nameof(newValue));
        EffectiveDateTime = effectiveDateTime;
        Status = "amended";
    }

    public void Cancel()
    {
        Status = "cancelled";
    }
}
