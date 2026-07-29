namespace eHIS.Application.DTOs;

public record PatientDto(
    string Id,
    bool Active,
    string? Gender,
    DateOnly? BirthDate,
    List<HumanNameDto> Names,
    List<ContactPointDto> Telecoms,
    List<AddressDto> Addresses,
    bool DeceasedBoolean,
    DateTime? DeceasedDateTime
);
