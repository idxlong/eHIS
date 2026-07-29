using FluentValidation;
using MediatR;
using eHIS.Application.DTOs;
using eHIS.Domain.Aggregates.EncounterAggregate;
using eHIS.Domain.Aggregates.ObservationAggregate;
using eHIS.Domain.Aggregates.PatientAggregate;
using eHIS.Domain.Aggregates.PractitionerAggregate;
using eHIS.Domain.ValueObjects;

namespace eHIS.Application.Observations;

public record RecordObservationCommand(
    string PatientId,
    string PerformerId,
    CodeableConceptDto Code,
    QuantityDto Value,
    DateTime EffectiveDateTime,
    string? EncounterId = null,
    List<CodeableConceptDto>? Category = null
) : IRequest<string>;

public class RecordObservationCommandHandler : IRequestHandler<RecordObservationCommand, string>
{
    private readonly IObservationRepository _observationRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IPractitionerRepository _practitionerRepository;
    private readonly IEncounterRepository _encounterRepository;

    public RecordObservationCommandHandler(
        IObservationRepository observationRepository,
        IPatientRepository patientRepository,
        IPractitionerRepository practitionerRepository,
        IEncounterRepository encounterRepository)
    {
        _observationRepository = observationRepository;
        _patientRepository = patientRepository;
        _practitionerRepository = practitionerRepository;
        _encounterRepository = encounterRepository;
    }

    public async Task<string> Handle(RecordObservationCommand request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(request.PatientId);
        if (patient == null)
        {
            throw new ArgumentException($"Patient with ID {request.PatientId} not found.");
        }

        var performer = await _practitionerRepository.GetByIdAsync(request.PerformerId);
        if (performer == null)
        {
            throw new ArgumentException($"Practitioner with ID {request.PerformerId} not found.");
        }

        if (!string.IsNullOrEmpty(request.EncounterId))
        {
            var encounter = await _encounterRepository.GetByIdAsync(request.EncounterId);
            if (encounter == null)
            {
                throw new ArgumentException($"Encounter with ID {request.EncounterId} not found.");
            }
        }

        var domainCode = new CodeableConcept(
            request.Code.Text,
            request.Code.Coding?.Select(c => new Coding(c.System, c.Code, c.Display, c.Version, c.UserSelected))
        );

        var domainValue = new Quantity(
            request.Value.Value,
            request.Value.Unit,
            request.Value.System,
            request.Value.Code,
            request.Value.Comparator
        );

        var domainCategories = request.Category?.Select(cat => new CodeableConcept(
            cat.Text,
            cat.Coding?.Select(c => new Coding(c.System, c.Code, c.Display, c.Version, c.UserSelected))
        )).ToList();

        var observation = new Observation(
            id: null,
            patientId: request.PatientId,
            performerId: request.PerformerId,
            code: domainCode,
            value: domainValue,
            effectiveDateTime: request.EffectiveDateTime,
            encounterId: request.EncounterId,
            categories: domainCategories
        );

        _observationRepository.Add(observation);
        await _observationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return observation.Id;
    }
}

public class RecordObservationCommandValidator : AbstractValidator<RecordObservationCommand>
{
    public RecordObservationCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithMessage("Patient ID is required.");
        RuleFor(x => x.PerformerId).NotEmpty().WithMessage("Performer (Practitioner) ID is required.");
        RuleFor(x => x.Code).NotNull().WithMessage("Observation Code is required.");
        RuleFor(x => x.Code.Text).NotEmpty().WithMessage("Observation Code Text is required.");
        RuleFor(x => x.Value).NotNull().WithMessage("Observation Value is required.");
        RuleFor(x => x.Value.Value).NotNull().WithMessage("Observation Numeric Value is required.");
    }
}
