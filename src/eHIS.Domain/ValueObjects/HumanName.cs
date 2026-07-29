using eHIS.Domain.SeedWork;

namespace eHIS.Domain.ValueObjects;

public class HumanName : ValueObject
{
    public string? Use { get; private set; }
    public string? Text { get; private set; }
    public string? Family { get; private set; }
    public List<string> Given { get; private set; } = new();
    public List<string> Prefix { get; private set; } = new();
    public List<string> Suffix { get; private set; } = new();
    public Period? Period { get; private set; }

    private HumanName() { } // EF Core

    public HumanName(
        string? family,
        IEnumerable<string>? given = null,
        string? text = null,
        string? use = null,
        IEnumerable<string>? prefix = null,
        IEnumerable<string>? suffix = null,
        Period? period = null)
    {
        Family = family;
        if (given != null) Given.AddRange(given);
        Text = text ?? string.Join(" ", Given.Concat(new[] { Family }).Where(s => !string.IsNullOrEmpty(s)));
        Use = use;
        if (prefix != null) Prefix.AddRange(prefix);
        if (suffix != null) Suffix.AddRange(suffix);
        Period = period;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Use;
        yield return Text;
        yield return Family;
        foreach (var g in Given) yield return g;
        foreach (var p in Prefix) yield return p;
        foreach (var s in Suffix) yield return s;
        yield return Period;
    }
}
