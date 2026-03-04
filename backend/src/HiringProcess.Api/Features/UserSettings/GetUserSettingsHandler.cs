using HiringProcess.Api.Common;
using HiringProcess.Api.Common.Localization;
using HiringProcess.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HiringProcess.Api.Features.UserSettings;

public sealed class GetUserSettingsHandler
{
    private readonly AppDbContext _db;
    private readonly ILocalizationService _loc;
    private readonly ICurrentLanguageService _currentLang;

    public GetUserSettingsHandler(AppDbContext db, ILocalizationService loc, ICurrentLanguageService currentLang)
    {
        _db = db;
        _loc = loc;
        _currentLang = currentLang;
    }

    public async Task<Result<UserSettingsResponse>> HandleAsync(GetUserSettingsQuery query, CancellationToken ct = default)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == query.UserId, ct);
        if (user is null)
            return Error.Custom("NotFound", _loc.Get("settings.userNotFound", _currentLang.Language));

        return new UserSettingsResponse(user.Language);
    }
}
