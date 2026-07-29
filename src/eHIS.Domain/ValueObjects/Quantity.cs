using eHIS.Domain.SeedWork;

namespace eHIS.Domain.ValueObjects;

public class Quantity : ValueObject
{
    public decimal? Value { get; private set; }
    public string? Comparator { get; private set; }
    public string? Unit { get; private set; }
    public string? System { get; private set; }
    public string? Code { get; private set; }

    private Quantity() { } // EF Core

    public Quantity(decimal? value, string? unit = null, string? system = null, string? code = null, string? comparator = null)
    {
        Value = value;
        Unit = unit;
        System = system;
        Code = code;
        Comparator = comparator;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
        yield return Comparator;
        yield return Unit;
        yield return System;
        yield return Code;
    }
}
