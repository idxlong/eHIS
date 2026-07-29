using MediatR;
using eHIS.Application.DTOs;
using eHIS.Domain.Aggregates.PractitionerAggregate;

namespace eHIS.Application.Practitioners;

public record GetPractitionerByIdQuery(string Id) : IRequest<PractitionerDto?>;

public class GetPractitionerByIdQueryHandler : IRequestHandler<GetPractitionerByIdQuery, PractitionerDto?>
{
    private readonly IPractitionerRepository _practitionerRepository;

    public GetPractitionerByIdQueryHandler(IPractitionerRepository practitionerRepository)
    {
        _practitionerRepository = practitionerRepository;
    }

    public async Task<PractitionerDto?> Handle(GetPractitionerByIdQuery request, CancellationToken cancellationToken)
    {
        var practitioner = await _practitionerRepository.GetByIdAsync(request.Id);
        if (practitioner == null)
            return null;

        return new PractitionerDto(
            practitioner.Id,
            practitioner.Active,
            practitioner.Gender,
            practitioner.BirthDate,
            practitioner.Names.Select(n => new HumanNameDto(
                n.Family,
                n.Given,
                n.Text,
                n.Use,
                n.Prefix,
                n.Suffix
            )).ToList(),
            practitioner.Telecoms.Select(t => new ContactPointDto(
                t.System,
                t.Value,
                t.Use,
                t.Rank
            )).ToList()
        );
    }
}
