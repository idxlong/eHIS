using FluentValidation;
using MediatR;
using eHIS.Application.DTOs;
using eHIS.Domain.Aggregates.PatientAggregate;
using eHIS.Domain.ValueObjects;

namespace eHIS.Application.Patients;

public record CreatePatientCommand(
    List<HumanNameDto> Names,
    string? Gender,
    DateOnly? BirthDate,
    List<ContactPointDto>? Telecoms,
    List<AddressDto>? Addresses
) : IRequest<string>;

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, string>
{
    private readonly IPatientRepository _patientRepository;

    public CreatePatientCommandHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<string> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var domainNames = request.Names.Select(n => new HumanName(
            n.Family,
            n.Given,
            n.Text,
            n.Use,
            n.Prefix,
            n.Suffix
        )).ToList();

        var domainTelecoms = request.Telecoms?.Select(t => new ContactPoint(
            t.System,
            t.Value,
            t.Use,
            t.Rank
        )).ToList();

        var domainAddresses = request.Addresses?.Select(a => new Address(
            a.Text,
            a.Line,
            a.City,
            a.District,
            a.State,
            a.PostalCode,
            a.Country,
            a.Use,
            a.Type
        )).ToList();

        var patient = new Patient(
            id: null, // Generated automatically
            names: domainNames,
            gender: request.Gender,
            birthDate: request.BirthDate,
            telecoms: domainTelecoms,
            addresses: domainAddresses
        );

        _patientRepository.Add(patient);
        await _patientRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return patient.Id;
    }
}

public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidator()
    {
        RuleFor(x => x.Names)
            .NotEmpty().WithMessage("Patient must have at least one name.");
        
        RuleFor(x => x.Names.First().Family)
            .NotEmpty().WithMessage("Family name is required.");

        RuleSet("GenderValidation", () =>
        {
            RuleFor(x => x.Gender)
                .Must(g => string.IsNullOrEmpty(g) || new[] { "male", "female", "other", "unknown" }.Contains(g.ToLower()))
                .WithMessage("Gender must be: male, female, other, or unknown.");
        });
    }
}
