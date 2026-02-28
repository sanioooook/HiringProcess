namespace HiringProcess.Api.Features.HiringProcesses.Commands;

/// <summary>
/// Command to create a new hiring process record for the current user.
/// </summary>
public sealed record CreateHiringProcessCommand(
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
