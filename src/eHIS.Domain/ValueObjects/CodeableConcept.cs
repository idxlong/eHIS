using eHIS.Domain.SeedWork;

namespace eHIS.Domain.ValueObjects;

public class CodeableConcept : ValueObject
{
    public List<Coding> Coding { get; private set; } = new();
    public string? Text { get; private set; }

    private CodeableConcept() { } // EF Core required constructor

    public CodeableConcept(string? text, IEnumerable<Coding>? coding = null)
    {
        Text = text;
        if (coding != null)
        {
            Coding.AddRange(coding);
        }
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Text;
        foreach (var c in Coding)
        {
            yield return c;
        }
    }
}
