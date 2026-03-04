using HiringProcess.Api.Common;
using HiringProcess.Api.Common.Localization;
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
    private readonly ILocalizationService _loc;
    private readonly ICurrentLanguageService _currentLang;

    public DownloadVacancyFileHandler(
        AppDbContext db,
        IFileStorageService fileStorage,
        ILocalizationService loc,
        ICurrentLanguageService currentLang)
    {
        _db = db;
        _fileStorage = fileStorage;
        _loc = loc;
        _currentLang = currentLang;
    }

    public async Task<Result<DownloadVacancyFileResult>> HandleAsync(
        DownloadVacancyFileQuery query,
        CancellationToken ct = default)
    {
        var lang = _currentLang.Language;

        var entity = await _db.HiringProcesses
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == query.HiringProcessId && h.UserId == query.UserId, ct);

        if (entity is null)
            return Error.Custom("NotFound", _loc.Get("hp.notFound", lang));

        if (entity.VacancyFileName is null)
            return Error.Custom("NoFile", _loc.Get("hp.noFile", lang));

        var file = await _fileStorage.GetAsync(entity.VacancyFileName, ct);

        if (file is null)
            return Error.Custom("FileNotFound", _loc.Get("hp.fileNotFound", lang));

        return new DownloadVacancyFileResult(file.Value.Stream, file.Value.ContentType, entity.VacancyFileName);
    }
}
