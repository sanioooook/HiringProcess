using HiringProcess.Api.Common;
using HiringProcess.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HiringProcess.Api.Features.HiringProcesses.Queries;

/// <summary>
/// Returns a single hiring process, verifying that it belongs to the requesting user.
/// </summary>
public sealed class GetHiringProcessByIdHandler
{
    private readonly AppDbContext _db;

    public GetHiringProcessByIdHandler(AppDbContext db) => _db = db;

    public async Task<Result<HiringProcessDto>> HandleAsync(
        GetHiringProcessByIdQuery query,
        CancellationToken ct = default)
    {
        var entity = await _db.HiringProcesses
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == query.Id && h.UserId == query.UserId, ct);

        if (entity is null)
            return Error.NotFound;

        return HiringProcessMapper.ToDto(entity);
    }
}
