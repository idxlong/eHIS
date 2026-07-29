using eHIS.Domain.SeedWork;

namespace eHIS.Domain.ValueObjects;

public class Address : ValueObject
{
    public string? Use { get; private set; } // home | work | temp | old | billing
    public string? Type { get; private set; } // postal | physical | both
    public string? Text { get; private set; }
    public List<string> Line { get; private set; } = new();
    public string? City { get; private set; }
    public string? District { get; private set; }
    public string? State { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Country { get; private set; }
    public Period? Period { get; private set; }

    private Address() { } // EF Core

    public Address(
        string? text,
        IEnumerable<string>? line = null,
        string? city = null,
        string? district = null,
        string? state = null,
        string? postalCode = null,
        string? country = null,
        string? use = null,
        string? type = null,
        Period? period = null)
    {
        Text = text;
        if (line != null) Line.AddRange(line);
        City = city;
        District = district;
        State = state;
        PostalCode = postalCode;
        Country = country;
        Use = use;
        Type = type;
        Period = period;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Use;
        yield return Type;
        yield return Text;
        foreach (var l in Line) yield return l;
        yield return City;
        yield return District;
        yield return State;
        yield return PostalCode;
        yield return Country;
        yield return Period;
    }
}
