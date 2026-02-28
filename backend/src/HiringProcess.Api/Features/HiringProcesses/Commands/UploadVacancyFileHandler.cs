using HiringProcess.Api.Common;
using HiringProcess.Api.Infrastructure;
using HiringProcess.Api.Infrastructure.FileStorage;
using Microsoft.EntityFrameworkCore;

namespace HiringProcess.Api.Features.HiringProcesses.Commands;

/// <summary>
/// Uploads a vacancy file (PDF or TXT) and links it to the hiring process.
/// If a file already exists, the old one is deleted before the new one is saved.
/// </summary>
public sealed class UploadVacancyFileHandler
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public UploadVacancyFileHandler(AppDbContext db, IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<Result<UploadVacancyFileResponse>> HandleAsync(
        UploadVacancyFileCommand command,
        CancellationToken ct = default)
    {
        // 1. Validate extension before touching storage
        var extension = Path.GetExtension(command.OriginalFileName).ToLowerInvariant();
        if (extension is not ".pdf" and not ".txt")
            return Error.Validation("Only PDF and TXT files are accepted.");

        // 2. Load entity with ownership check
        var entity = await _db.HiringProcesses
            .FirstOrDefaultAsync(h => h.Id == command.HiringProcessId && h.UserId == command.UserId, ct);

        if (entity is null)
            return Error.NotFound;

        // 3. Delete old file if present
        if (entity.VacancyFileName is not null)
            await _fileStorage.DeleteAsync(entity.VacancyFileName, ct);

        // 4. Save new file
        var storedName = await _fileStorage.SaveAsync(command.FileStream, command.OriginalFileName, ct);

        // 5. Update entity
        entity.VacancyFileName = storedName;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        return new UploadVacancyFileResponse(storedName);
    }
}
