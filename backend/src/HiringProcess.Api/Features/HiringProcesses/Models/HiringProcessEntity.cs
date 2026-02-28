namespace HiringProcess.Api.Features.HiringProcesses.Models;

/// <summary>
/// Core domain entity representing a single job application tracking record.
/// All stage/channel/applied-with values are free-form strings — no FK lookups.
/// </summary>
public sealed class HiringProcessEntity
{
    public Guid Id { get; set; }

    // Owner — every record is private to the user who created it
    public Guid UserId { get; set; }

    // Required fields (enforced at DB level and via FluentValidation)
    public string CompanyName { get; set; } = string.Empty;
    public string ContactChannel { get; set; } = string.Empty;

    // Optional contact info
    public string? ContactPerson { get; set; }

    // Timeline dates — all optional at DB level; business rules applied in handlers
    public DateOnly? FirstContactDate { get; set; }
    public DateOnly? LastContactDate { get; set; }
    public DateOnly? VacancyPublishedDate { get; set; }
    public DateOnly? ApplicationDate { get; set; }

    // Application details
    public string? AppliedWith { get; set; }
    public string? AppliedLink { get; set; }
    public string? CoverLetter { get; set; }
    public string? SalaryRange { get; set; }

    // Pipeline tracking — stored as comma-separated string in DB, projected to List<string>
    public string HiringStagesRaw { get; set; } = string.Empty;
    public string? CurrentStage { get; set; }

    // Vacancy info
    public string? VacancyLink { get; set; }
    public string? VacancyFileName { get; set; } // stored path/name of the uploaded file
    public string? VacancyText { get; set; }
    public string? Notes { get; set; }

    // Audit — stored as UTC DateTime to ensure SQLite and PostgreSQL compatibility.
    // Exposed as DateTimeOffset in the DTO layer.
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Computed — not mapped to a DB column
    public List<string> HiringStages
    {
        get => string.IsNullOrWhiteSpace(HiringStagesRaw)
            ? []
            : [.. HiringStagesRaw.Split('|', StringSplitOptions.RemoveEmptyEntries)];
        set => HiringStagesRaw = value.Count > 0 ? string.Join('|', value) : string.Empty;
    }
}
