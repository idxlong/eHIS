namespace eHIS.Application.DTOs;

public record PractitionerDto(
    string Id,
    bool Active,
    string? Gender,
    DateOnly? BirthDate,
    List<HumanNameDto> Names,
    List<ContactPointDto> Telecoms
);
