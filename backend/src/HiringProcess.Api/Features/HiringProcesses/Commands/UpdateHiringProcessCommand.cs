namespace HiringProcess.Api.Features.HiringProcesses.Commands;

/// <summary>
/// Command to update all mutable fields of an existing hiring process.
/// UserId is used to enforce ownership — users cannot modify other users' records.
/// </summary>
public sealed record UpdateHiringProcessCommand(
    Guid Id,
    Guid UserId,
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
    List<string>? HiringStages,
    string? CurrentStage,
    string? VacancyLink,
    string? VacancyText,
    string? Notes
);
