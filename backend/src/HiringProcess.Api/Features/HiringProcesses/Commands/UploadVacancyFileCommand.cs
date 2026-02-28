namespace HiringProcess.Api.Features.HiringProcesses.Commands;

/// <summary>
/// Command to upload or replace the vacancy file attached to a hiring process.
/// </summary>
public sealed record UploadVacancyFileCommand(
    Guid HiringProcessId,
    Guid UserId,
    Stream FileStream,
    string OriginalFileName
);

/// <summary>
/// Response after a successful file upload.
/// </summary>
public sealed record UploadVacancyFileResponse(string StoredFileName);
