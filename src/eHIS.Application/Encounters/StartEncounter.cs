using FluentValidation;
using MediatR;
using eHIS.Application.DTOs;
using eHIS.Domain.Aggregates.EncounterAggregate;
using eHIS.Domain.Aggregates.PatientAggregate;
using eHIS.Domain.Aggregates.PractitionerAggregate;
using eHIS.Domain.ValueObjects;

namespace eHIS.Application.Encounters;

public record StartEncounterCommand(
    string PatientId,
    string PractitionerId,
    CodingDto Class,
    DateTime StartTime
) : IRequest<string>;

public class StartEncounterCommandHandler : IRequestHandler<StartEncounterCommand, string>
{
    private readonly IEncounterRepository _encounterRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IPractitionerRepository _practitionerRepository;

    public StartEncounterCommandHandler(
        IEncounterRepository encounterRepository,
        IPatientRepository patientRepository,
        IPractitionerRepository practitionerRepository)
    {
        _encounterRepository = encounterRepository;
        _patientRepository = patientRepository;
        _practitionerRepository = practitionerRepository;
    }

    public async Task<string> Handle(StartEncounterCommand request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(request.PatientId);
        if (patient == null)
        {
            throw new ArgumentException($"Patient with ID {request.PatientId} not found.");
        }

        var practitioner = await _practitionerRepository.GetByIdAsync(request.PractitionerId);
        if (practitioner == null)
        {
            throw new ArgumentException($"Practitioner with ID {request.PractitionerId} not found.");
        }

        var domainClass = new Coding(
            request.Class.System,
            request.Class.Code,
            request.Class.Display,
            request.Class.Version,
            request.Class.UserSelected
        );

        var encounter = new Encounter(
            id: null,
            patientId: request.PatientId,
            practitionerId: request.PractitionerId,
            @class: domainClass,
            startTime: request.StartTime
        );

        _encounterRepository.Add(encounter);
        await _encounterRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return encounter.Id;
    }
}

public class StartEncounterCommandValidator : AbstractValidator<StartEncounterCommand>
{
    public StartEncounterCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithMessage("Patient ID is required.");
        RuleFor(x => x.PractitionerId).NotEmpty().WithMessage("Practitioner ID is required.");
        RuleFor(x => x.Class).NotNull().WithMessage("Encounter Class is required.");
        RuleFor(x => x.Class.Code).NotEmpty().WithMessage("Encounter Class Code is required.");
    }
}
