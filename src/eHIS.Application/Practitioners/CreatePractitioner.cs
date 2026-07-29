using FluentValidation;
using MediatR;
using eHIS.Application.DTOs;
using eHIS.Domain.Aggregates.PractitionerAggregate;
using eHIS.Domain.ValueObjects;

namespace eHIS.Application.Practitioners;

public record CreatePractitionerCommand(
    List<HumanNameDto> Names,
    string? Gender,
    DateOnly? BirthDate,
    List<ContactPointDto>? Telecoms
) : IRequest<string>;

public class CreatePractitionerCommandHandler : IRequestHandler<CreatePractitionerCommand, string>
{
    private readonly IPractitionerRepository _practitionerRepository;

    public CreatePractitionerCommandHandler(IPractitionerRepository practitionerRepository)
    {
        _practitionerRepository = practitionerRepository;
    }

    public async Task<string> Handle(CreatePractitionerCommand request, CancellationToken cancellationToken)
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

        var practitioner = new Practitioner(
            id: null,
            names: domainNames,
            gender: request.Gender,
            birthDate: request.BirthDate,
            telecoms: domainTelecoms
        );

        _practitionerRepository.Add(practitioner);
        await _practitionerRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return practitioner.Id;
    }
}

public class CreatePractitionerCommandValidator : AbstractValidator<CreatePractitionerCommand>
{
    public CreatePractitionerCommandValidator()
    {
        RuleFor(x => x.Names)
            .NotEmpty().WithMessage("Practitioner must have at least one name.");
        
        RuleFor(x => x.Names.First().Family)
            .NotEmpty().WithMessage("Family name is required.");
    }
}
