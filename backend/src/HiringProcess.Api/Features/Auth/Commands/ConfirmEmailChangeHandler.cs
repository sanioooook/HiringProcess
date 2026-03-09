using HiringProcess.Api.Common;
using HiringProcess.Api.Common.Localization;
using HiringProcess.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HiringProcess.Api.Features.Auth.Commands;

public sealed record ConfirmEmailChangeCommand(string Token);

/// <summary>
/// Confirms an email change by swapping Email ← PendingEmail using the token sent to the new address.
/// </summary>
public sealed class ConfirmEmailChangeHandler
{
    private readonly AppDbContext _db;
    private readonly ILocalizationService _loc;
    private readonly ICurrentLanguageService _currentLang;

    public ConfirmEmailChangeHandler(AppDbContext db, ILocalizationService loc, ICurrentLanguageService currentLang)
    {
        _db = db;
        _loc = loc;
        _currentLang = currentLang;
    }

    public async Task<Result> HandleAsync(ConfirmEmailChangeCommand command, CancellationToken ct = default)
    {
        var lang = _currentLang.Language;

        if (string.IsNullOrWhiteSpace(command.Token))
            return Error.Validation(_loc.Get("auth.tokenRequired", lang));

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.EmailChangeToken == command.Token, ct);

        if (user is null || user.EmailChangeTokenExpiry < DateTime.UtcNow || user.PendingEmail is null)
            return Error.Custom("InvalidToken", _loc.Get("auth.tokenInvalidOrExpired", lang));

        user.Email = user.PendingEmail;
        user.PendingEmail = null;
        user.EmailChangeToken = null;
        user.EmailChangeTokenExpiry = null;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
