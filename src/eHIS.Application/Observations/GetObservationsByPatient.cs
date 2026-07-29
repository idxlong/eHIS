using MediatR;
using eHIS.Application.DTOs;
using eHIS.Domain.Aggregates.ObservationAggregate;

namespace eHIS.Application.Observations;

public record GetObservationsByPatientQuery(string PatientId) : IRequest<List<ObservationDto>>;

public class GetObservationsByPatientQueryHandler : IRequestHandler<GetObservationsByPatientQuery, List<ObservationDto>>
{
    private readonly IObservationRepository _observationRepository;

    public GetObservationsByPatientQueryHandler(IObservationRepository observationRepository)
    {
        _observationRepository = observationRepository;
    }

    public async Task<List<ObservationDto>> Handle(GetObservationsByPatientQuery request, CancellationToken cancellationToken)
    {
        var observations = await _observationRepository.GetByPatientIdAsync(request.PatientId);

        return observations.Select(obs => new ObservationDto(
            obs.Id,
            obs.Status,
            obs.Category.Select(cat => new CodeableConceptDto(cat.Text, cat.Coding.Select(c => new CodingDto(c.System, c.Code, c.Display, c.Version, c.UserSelected)).ToList())).ToList(),
            new CodeableConceptDto(obs.Code.Text, obs.Code.Coding.Select(c => new CodingDto(c.System, c.Code, c.Display, c.Version, c.UserSelected)).ToList()),
            obs.PatientId,
            obs.EncounterId,
            obs.EffectiveDateTime,
            new QuantityDto(obs.Value.Value, obs.Value.Unit, obs.Value.System, obs.Value.Code, obs.Value.Comparator),
            obs.PerformerId
        )).ToList();
    }
}
