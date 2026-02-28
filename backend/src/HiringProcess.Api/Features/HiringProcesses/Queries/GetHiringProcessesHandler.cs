using HiringProcess.Api.Common;
using HiringProcess.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HiringProcess.Api.Features.HiringProcesses.Queries;

/// <summary>
/// Returns a paginated, sorted, filtered list of hiring processes for the current user.
/// </summary>
public sealed class GetHiringProcessesHandler
{
    private readonly AppDbContext _db;

    public GetHiringProcessesHandler(AppDbContext db) => _db = db;

    public async Task<Result<PagedResult<HiringProcessDto>>> HandleAsync(
        GetHiringProcessesQuery query,
        CancellationToken ct = default)
    {
        if (query.Page < 1 || query.PageSize < 1 || query.PageSize > 100)
            return Error.Validation("Page must be >= 1 and PageSize must be between 1 and 100.");

        var q = _db.HiringProcesses
            .AsNoTracking()
            .Where(h => h.UserId == query.UserId);

        // Filter by search term — use EF.Functions.Like for case-insensitive search
        // that works on both PostgreSQL (case-insensitive collation) and SQLite (LIKE is case-insensitive for ASCII).
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var pattern = $"%{query.SearchTerm}%";
            q = q.Where(h =>
                EF.Functions.Like(h.CompanyName, pattern) ||
                (h.ContactPerson != null && EF.Functions.Like(h.ContactPerson, pattern)) ||
                (h.CurrentStage != null && EF.Functions.Like(h.CurrentStage, pattern)));
        }

        // Filter by current stage
        if (!string.IsNullOrWhiteSpace(query.CurrentStage))
        {
            var stage = query.CurrentStage;
            q = q.Where(h => h.CurrentStage == stage);
        }

        var totalCount = await q.CountAsync(ct);

        // Dynamic sort
        q = (query.SortBy.ToLowerInvariant(), query.SortDirection.ToLowerInvariant()) switch
        {
            ("companyname", "asc") => q.OrderBy(h => h.CompanyName),
            ("companyname", _) => q.OrderByDescending(h => h.CompanyName),
            ("applicationdate", "asc") => q.OrderBy(h => h.ApplicationDate),
            ("applicationdate", _) => q.OrderByDescending(h => h.ApplicationDate),
            ("currentstage", "asc") => q.OrderBy(h => h.CurrentStage),
            ("currentstage", _) => q.OrderByDescending(h => h.CurrentStage),
            ("updatedat", "asc") => q.OrderBy(h => h.UpdatedAt),
            ("updatedat", _) => q.OrderByDescending(h => h.UpdatedAt),
            (_, "asc")  => q.OrderBy(h => h.CreatedAt),
            _ => q.OrderByDescending(h => h.CreatedAt)
        };

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        var dtos = items.Select(HiringProcessMapper.ToDto).ToList();

        return new PagedResult<HiringProcessDto>(dtos, totalCount, query.Page, query.PageSize);
    }
}
