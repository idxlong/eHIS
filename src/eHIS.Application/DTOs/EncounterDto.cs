namespace eHIS.Application.DTOs;

public record EncounterDiagnosisDto(CodeableConceptDto Condition, CodingDto? Use, int? Rank);

public record EncounterDto(
    string Id,
    string Status,
    CodingDto Class,
    string PatientId,
    string PractitionerId,
    PeriodDto Period,
    List<EncounterDiagnosisDto> Diagnoses
);
