using FluentValidation;
using MediatR;
using eHIS.Application.DTOs;
using eHIS.Domain.Aggregates.EncounterAggregate;
using eHIS.Domain.ValueObjects;

namespace eHIS.Application.Encounters;

public record AddEncounterDiagnosisCommand(
    string EncounterId,
    CodeableConceptDto Condition,
    CodingDto? Use,
    int? Rank
) : IRequest<bool>;

public class AddEncounterDiagnosisCommandHandler : IRequestHandler<AddEncounterDiagnosisCommand, bool>
{
    private readonly IEncounterRepository _encounterRepository;

    public AddEncounterDiagnosisCommandHandler(IEncounterRepository encounterRepository)
    {
        _encounterRepository = encounterRepository;
    }

    public async Task<bool> Handle(AddEncounterDiagnosisCommand request, CancellationToken cancellationToken)
    {
        var encounter = await _encounterRepository.GetByIdAsync(request.EncounterId);
        if (encounter == null)
        {
            throw new ArgumentException($"Encounter with ID {request.EncounterId} not found.");
        }

        var domainCondition = new CodeableConcept(
            request.Condition.Text,
            request.Condition.Coding?.Select(c => new Coding(c.System, c.Code, c.Display, c.Version, c.UserSelected))
        );

        var domainUse = request.Use != null ? new Coding(
            request.Use.System,
            request.Use.Code,
            request.Use.Display,
            request.Use.Version,
            request.Use.UserSelected
        ) : null;

        encounter.AddDiagnosis(domainCondition, domainUse, request.Rank);
        
        _encounterRepository.Update(encounter);
        await _encounterRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return true;
    }
}

public class AddEncounterDiagnosisCommandValidator : AbstractValidator<AddEncounterDiagnosisCommand>
{
    public AddEncounterDiagnosisCommandValidator()
    {
        RuleFor(x => x.EncounterId).NotEmpty().WithMessage("Encounter ID is required.");
        RuleFor(x => x.Condition).NotNull().WithMessage("Condition is required.");
        RuleFor(x => x.Condition.Text).NotEmpty().WithMessage("Condition Text is required.");
    }
}
