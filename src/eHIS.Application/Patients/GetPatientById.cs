using MediatR;
using eHIS.Application.DTOs;
using eHIS.Domain.Aggregates.PatientAggregate;

namespace eHIS.Application.Patients;

public record GetPatientByIdQuery(string Id) : IRequest<PatientDto?>;

public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, PatientDto?>
{
    private readonly IPatientRepository _patientRepository;

    public GetPatientByIdQueryHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PatientDto?> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(request.Id);
        if (patient == null)
            return null;

        return new PatientDto(
            patient.Id,
            patient.Active,
            patient.Gender,
            patient.BirthDate,
            patient.Names.Select(n => new HumanNameDto(
                n.Family,
                n.Given,
                n.Text,
                n.Use,
                n.Prefix,
                n.Suffix
            )).ToList(),
            patient.Telecoms.Select(t => new ContactPointDto(
                t.System,
                t.Value,
                t.Use,
                t.Rank
            )).ToList(),
            patient.Addresses.Select(a => new AddressDto(
                a.Text,
                a.Line,
                a.City,
                a.District,
                a.State,
                a.PostalCode,
                a.Country,
                a.Use,
                a.Type
            )).ToList(),
            patient.DeceasedBoolean,
            patient.DeceasedDateTime
        );
    }
}
