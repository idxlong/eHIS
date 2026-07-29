using eHIS.Domain.SeedWork;
using eHIS.Domain.ValueObjects;

namespace eHIS.Domain.Aggregates.EncounterAggregate;

public class EncounterDiagnosis : Entity<int>
{
    public CodeableConcept Condition { get; private set; } = null!;
    public Coding? Use { get; private set; } // e.g. admission | discharge | billing
    public int? Rank { get; private set; }

    private EncounterDiagnosis() { } // For EF Core

    public EncounterDiagnosis(CodeableConcept condition, Coding? use = null, int? rank = null)
    {
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        Use = use;
        Rank = rank;
    }
}
