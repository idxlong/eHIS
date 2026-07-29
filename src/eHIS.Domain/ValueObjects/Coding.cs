using eHIS.Domain.SeedWork;

namespace eHIS.Domain.ValueObjects;

public class Coding : ValueObject
{
    public string? System { get; private set; }
    public string? Version { get; private set; }
    public string? Code { get; private set; }
    public string? Display { get; private set; }
    public bool? UserSelected { get; private set; }

    private Coding() { } // EF Core required constructor

    public Coding(string? system, string? code, string? display = null, string? version = null, bool? userSelected = null)
    {
        System = system;
        Code = code;
        Display = display;
        Version = version;
        UserSelected = userSelected;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return System;
        yield return Code;
        yield return Display;
        yield return Version;
        yield return UserSelected;
    }
}
