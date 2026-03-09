using HiringProcess.Api.Common;
using HiringProcess.Api.Infrastructure;
using HiringProcess.Api.Infrastructure.FileStorage;
using Microsoft.EntityFrameworkCore;

namespace HiringProcess.Api.Features.HiringProcesses.Commands;

/// <summary>
/// Deletes a hiring process and its associated vacancy file (if any).
/// Returns NotFound if the record does not exist or belongs to another user.
/// </summary>
public sealed class DeleteHiringProcessHandler
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public DeleteHiringProcessHandler(AppDbContext db, IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<Result> HandleAsync(DeleteHiringProcessCommand command, CancellationToken ct = default)
    {
        // Load entity - ownership enforced here
        var entity = await _db.HiringProcesses
            .FirstOrDefaultAsync(h => h.Id == command.Id && h.UserId == command.UserId, ct);

        if (entity is null)
            return Error.NotFound;

        // Delete associated file if present
        if (entity.VacancyFileName is not null)
            await _fileStorage.DeleteAsync(entity.VacancyFileName, ct);

        // Remove from DB
        _db.HiringProcesses.Remove(entity);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
