using eHIS.Domain.Events;
using eHIS.Domain.SeedWork;
using eHIS.Domain.ValueObjects;

namespace eHIS.Domain.Aggregates.PatientAggregate;

public class Patient : AggregateRoot<string>
{
    public bool Active { get; private set; }
    public string? Gender { get; private set; } // male | female | other | unknown
    public DateOnly? BirthDate { get; private set; }
    public bool DeceasedBoolean { get; private set; }
    public DateTime? DeceasedDateTime { get; private set; }

    private readonly List<HumanName> _names = new();
    public IReadOnlyCollection<HumanName> Names => _names.AsReadOnly();

    private readonly List<ContactPoint> _telecoms = new();
    public IReadOnlyCollection<ContactPoint> Telecoms => _telecoms.AsReadOnly();

    private readonly List<Address> _addresses = new();
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    private Patient() { } // For EF Core

    public Patient(
        string? id,
        IEnumerable<HumanName> names,
        string? gender = null,
        DateOnly? birthDate = null,
        IEnumerable<ContactPoint>? telecoms = null,
        IEnumerable<Address>? addresses = null)
    {
        Id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;
        Active = true;
        Gender = gender;
        BirthDate = birthDate;

        if (names == null || !names.Any())
        {
            throw new ArgumentException("Patient must have at least one name.");
        }
        _names.AddRange(names);

        if (telecoms != null)
        {
            _telecoms.AddRange(telecoms);
        }

        if (addresses != null)
        {
            _addresses.AddRange(addresses);
        }

        var primaryNameText = _names.FirstOrDefault(n => n.Use == "official")?.Text ?? _names.First().Text;
        AddDomainEvent(new PatientCreatedDomainEvent(Id, primaryNameText));
    }

    public void UpdateDemographics(
        IEnumerable<HumanName> names,
        string? gender,
        DateOnly? birthDate,
        IEnumerable<ContactPoint> telecoms,
        IEnumerable<Address> addresses)
    {
        if (names == null || !names.Any())
        {
            throw new ArgumentException("Patient must have at least one name.");
        }

        _names.Clear();
        _names.AddRange(names);

        Gender = gender;
        BirthDate = birthDate;

        _telecoms.Clear();
        _telecoms.AddRange(telecoms);

        _addresses.Clear();
        _addresses.AddRange(addresses);

        var primaryNameText = _names.FirstOrDefault(n => n.Use == "official")?.Text ?? _names.First().Text;
        AddDomainEvent(new PatientDemographicsUpdatedDomainEvent(Id, primaryNameText));
    }

    public void Deactivate()
    {
        if (Active)
        {
            Active = false;
            AddDomainEvent(new PatientDeactivatedDomainEvent(Id));
        }
    }

    public void MarkDeceased(DateTime deceasedDateTime)
    {
        DeceasedBoolean = true;
        DeceasedDateTime = deceasedDateTime;
    }

    public void MarkDeceased(bool deceased)
    {
        DeceasedBoolean = deceased;
        if (!deceased)
        {
            DeceasedDateTime = null;
        }
    }
}
