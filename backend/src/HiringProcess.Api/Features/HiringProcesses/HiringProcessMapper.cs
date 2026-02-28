using HiringProcess.Api.Features.HiringProcesses.Models;

namespace HiringProcess.Api.Features.HiringProcesses;

/// <summary>
/// Maps entity to DTO. Kept as a static helper to avoid pulling in AutoMapper.
/// </summary>
internal static class HiringProcessMapper
{
    public static HiringProcessDto ToDto(HiringProcessEntity entity) => new(
        entity.Id,
        entity.CompanyName,
        entity.ContactChannel,
        entity.ContactPerson,
        entity.FirstContactDate,
        entity.LastContactDate,
        entity.VacancyPublishedDate,
        entity.ApplicationDate,
        entity.AppliedWith,
        entity.AppliedLink,
        entity.CoverLetter,
        entity.SalaryRange,
        entity.HiringStages,
        entity.CurrentStage,
        entity.VacancyLink,
        HasVacancyFile: entity.VacancyFileName is not null,
        entity.VacancyText,
        entity.Notes,
        entity.CreatedAt,
        entity.UpdatedAt
    );
}
