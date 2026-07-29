using FluentValidation;
using MediatR;
using eHIS.Domain.Aggregates.EncounterAggregate;

namespace eHIS.Application.Encounters;

public record CompleteEncounterCommand(
    string EncounterId,
    DateTime EndTime
) : IRequest<bool>;

public class CompleteEncounterCommandHandler : IRequestHandler<CompleteEncounterCommand, bool>
{
    private readonly IEncounterRepository _encounterRepository;

    public CompleteEncounterCommandHandler(IEncounterRepository encounterRepository)
    {
        _encounterRepository = encounterRepository;
    }

    public async Task<bool> Handle(CompleteEncounterCommand request, CancellationToken cancellationToken)
    {
        var encounter = await _encounterRepository.GetByIdAsync(request.EncounterId);
        if (encounter == null)
        {
            throw new ArgumentException($"Encounter with ID {request.EncounterId} not found.");
        }

        encounter.CompleteEncounter(request.EndTime);

        _encounterRepository.Update(encounter);
        await _encounterRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return true;
    }
}

public class CompleteEncounterCommandValidator : AbstractValidator<CompleteEncounterCommand>
{
    public CompleteEncounterCommandValidator()
    {
        RuleFor(x => x.EncounterId).NotEmpty().WithMessage("Encounter ID is required.");
        RuleFor(x => x.EndTime).NotEmpty().WithMessage("End time is required.");
    }
}
