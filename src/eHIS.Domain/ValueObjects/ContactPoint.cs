using eHIS.Domain.SeedWork;

namespace eHIS.Domain.ValueObjects;

public class ContactPoint : ValueObject
{
    public string? System { get; private set; } // phone | fax | email | pager | url | sms | other
    public string? Value { get; private set; }
    public string? Use { get; private set; } // home | work | temp | old | mobile
    public int? Rank { get; private set; }
    public Period? Period { get; private set; }

    private ContactPoint() { } // EF Core

    public ContactPoint(string? system, string? value, string? use = null, int? rank = null, Period? period = null)
    {
        System = system;
        Value = value;
        Use = use;
        Rank = rank;
        Period = period;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return System;
        yield return Value;
        yield return Use;
        yield return Rank;
        yield return Period;
    }
}
