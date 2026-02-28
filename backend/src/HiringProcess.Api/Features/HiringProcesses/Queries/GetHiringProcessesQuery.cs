namespace HiringProcess.Api.Features.HiringProcesses.Queries;

/// <summary>
/// Parameters for the paginated/sorted/filtered hiring-processes list query.
/// </summary>
public sealed record GetHiringProcessesQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 20,
    string? SearchTerm = null, // searched across CompanyName, ContactPerson, CurrentStage
    string? CurrentStage = null,
    string SortBy = "CreatedAt",
    string SortDirection = "desc"
);
