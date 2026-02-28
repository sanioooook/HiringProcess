using HiringProcess.Api.Common;
using HiringProcess.Api.Infrastructure;
using HiringProcess.Api.Infrastructure.FileStorage;
using Microsoft.EntityFrameworkCore;

namespace HiringProcess.Api.Features.HiringProcesses.Queries;

/// <summary>
/// Retrieves the stored vacancy file for a hiring process.
/// Verifies record ownership before streaming the file.
/// </summary>
public sealed class DownloadVacancyFileHandler
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public DownloadVacancyFileHandler(AppDbContext db, IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<Result<DownloadVacancyFileResult>> HandleAsync(
        DownloadVacancyFileQuery query,
        CancellationToken ct = default)
    {
        var entity = await _db.HiringProcesses
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == query.HiringProcessId && h.UserId == query.UserId, ct);

        if (entity is null)
            return Error.NotFound;

        if (entity.VacancyFileName is null)
            return Error.Custom("NoFile", "This hiring process has no attached vacancy file.");

        var file = await _fileStorage.GetAsync(entity.VacancyFileName, ct);

        if (file is null)
            return Error.Custom("FileNotFound", "The vacancy file could not be found in storage.");

        return new DownloadVacancyFileResult(file.Value.Stream, file.Value.ContentType, entity.VacancyFileName);
    }
}
