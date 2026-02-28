namespace HiringProcess.Api.Features.HiringProcesses;

/// <summary>
/// Shared read model returned from all GET endpoints.
/// </summary>
public sealed record HiringProcessDto(
    Guid Id,
    string CompanyName,
    string ContactChannel,
    string? ContactPerson,
    DateOnly? FirstContactDate,
    DateOnly? LastContactDate,
    DateOnly? VacancyPublishedDate,
    DateOnly? ApplicationDate,
    string? AppliedWith,
    string? AppliedLink,
    string? CoverLetter,
    string? SalaryRange,
    List<string> HiringStages,
    string? CurrentStage,
    string? VacancyLink,
    bool HasVacancyFile,
    string? VacancyText,
    string? Notes,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
