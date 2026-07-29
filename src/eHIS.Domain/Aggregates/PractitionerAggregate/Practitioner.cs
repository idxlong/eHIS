using eHIS.Domain.Events;
using eHIS.Domain.SeedWork;
using eHIS.Domain.ValueObjects;

namespace eHIS.Domain.Aggregates.PractitionerAggregate;

public class Practitioner : AggregateRoot<string>
{
    public bool Active { get; private set; }
    public string? Gender { get; private set; }
    public DateOnly? BirthDate { get; private set; }

    private readonly List<HumanName> _names = new();
    public IReadOnlyCollection<HumanName> Names => _names.AsReadOnly();

    private readonly List<ContactPoint> _telecoms = new();
    public IReadOnlyCollection<ContactPoint> Telecoms => _telecoms.AsReadOnly();

    private Practitioner() { } // For EF Core

    public Practitioner(
        string? id,
        IEnumerable<HumanName> names,
        string? gender = null,
        DateOnly? birthDate = null,
        IEnumerable<ContactPoint>? telecoms = null)
    {
        Id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;
        Active = true;
        Gender = gender;
        BirthDate = birthDate;

        if (names == null || !names.Any())
        {
            throw new ArgumentException("Practitioner must have at least one name.");
        }
        _names.AddRange(names);

        if (telecoms != null)
        {
            _telecoms.AddRange(telecoms);
        }

        var primaryNameText = _names.FirstOrDefault(n => n.Use == "official")?.Text ?? _names.First().Text;
        AddDomainEvent(new PractitionerCreatedDomainEvent(Id, primaryNameText));
    }

    public void UpdateDemographics(
        IEnumerable<HumanName> names,
        string? gender,
        DateOnly? birthDate,
        IEnumerable<ContactPoint> telecoms)
    {
        if (names == null || !names.Any())
        {
            throw new ArgumentException("Practitioner must have at least one name.");
        }

        _names.Clear();
        _names.AddRange(names);

        Gender = gender;
        BirthDate = birthDate;

        _telecoms.Clear();
        _telecoms.AddRange(telecoms);
    }

    public void Deactivate()
    {
        Active = false;
    }
}
