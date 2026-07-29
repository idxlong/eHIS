namespace eHIS.Application.DTOs;

public record ObservationDto(
    string Id,
    string Status,
    List<CodeableConceptDto> Category,
    CodeableConceptDto Code,
    string PatientId,
    string? EncounterId,
    DateTime EffectiveDateTime,
    QuantityDto Value,
    string PerformerId
);
