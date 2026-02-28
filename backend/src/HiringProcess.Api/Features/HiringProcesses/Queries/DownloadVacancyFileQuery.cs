namespace HiringProcess.Api.Features.HiringProcesses.Queries;

/// <summary>
/// Query to download the vacancy file attached to a hiring process.
/// </summary>
public sealed record DownloadVacancyFileQuery(Guid HiringProcessId, Guid UserId);

/// <summary>
/// Result of a successful file download query.
/// </summary>
public sealed record DownloadVacancyFileResult(
    Stream FileStream,
    string ContentType,
    string FileName
);
