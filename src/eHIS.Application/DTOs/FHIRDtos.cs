namespace eHIS.Application.DTOs;

public record CodingDto(string? System, string? Code, string? Display, string? Version = null, bool? UserSelected = null);

public record CodeableConceptDto(string? Text, List<CodingDto>? Coding);

public record QuantityDto(decimal? Value, string? Unit, string? System, string? Code, string? Comparator = null);

public record PeriodDto(DateTime? Start, DateTime? End);

public record HumanNameDto(string? Family, List<string>? Given, string? Text = null, string? Use = null, List<string>? Prefix = null, List<string>? Suffix = null);

public record ContactPointDto(string? System, string? Value, string? Use = null, int? Rank = null);

public record AddressDto(string? Text, List<string>? Line, string? City, string? District, string? State, string? PostalCode, string? Country, string? Use = null, string? Type = null);
