using MediatR;
using eHIS.Application.DTOs;
using eHIS.Domain.Aggregates.EncounterAggregate;

namespace eHIS.Application.Encounters;

public record GetEncounterByIdQuery(string Id) : IRequest<EncounterDto?>;

public class GetEncounterByIdQueryHandler : IRequestHandler<GetEncounterByIdQuery, EncounterDto?>
{
    private readonly IEncounterRepository _encounterRepository;

    public GetEncounterByIdQueryHandler(IEncounterRepository encounterRepository)
    {
        _encounterRepository = encounterRepository;
    }

    public async Task<EncounterDto?> Handle(GetEncounterByIdQuery request, CancellationToken cancellationToken)
    {
        var encounter = await _encounterRepository.GetByIdAsync(request.Id);
        if (encounter == null)
            return null;

        return new EncounterDto(
            encounter.Id,
            encounter.Status,
            new CodingDto(encounter.Class.System, encounter.Class.Code, encounter.Class.Display, encounter.Class.Version, encounter.Class.UserSelected),
            encounter.PatientId,
            encounter.PractitionerId,
            new PeriodDto(encounter.Period.Start, encounter.Period.End),
            encounter.Diagnoses.Select(d => new EncounterDiagnosisDto(
                new CodeableConceptDto(d.Condition.Text, d.Condition.Coding.Select(c => new CodingDto(c.System, c.Code, c.Display, c.Version, c.UserSelected)).ToList()),
                d.Use != null ? new CodingDto(d.Use.System, d.Use.Code, d.Use.Display, d.Use.Version, d.Use.UserSelected) : null,
                d.Rank
            )).ToList()
        );
    }
}
