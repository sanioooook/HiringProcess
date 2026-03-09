using HiringProcess.Api.Common;
using HiringProcess.Api.Common.Localization;
using HiringProcess.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HiringProcess.Api.Features.Auth.Commands;

public sealed record VerifyEmailCommand(string Token);

/// <summary>
/// Marks a user's email as verified using the token sent during registration.
/// </summary>
public sealed class VerifyEmailHandler
{
    private readonly AppDbContext _db;
    private readonly ILocalizationService _loc;
    private readonly ICurrentLanguageService _currentLang;

    public VerifyEmailHandler(AppDbContext db, ILocalizationService loc, ICurrentLanguageService currentLang)
    {
        _db = db;
        _loc = loc;
        _currentLang = currentLang;
    }

    public async Task<Result> HandleAsync(VerifyEmailCommand command, CancellationToken ct = default)
    {
        var lang = _currentLang.Language;

        if (string.IsNullOrWhiteSpace(command.Token))
            return Error.Validation(_loc.Get("auth.tokenRequired", lang));

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.EmailVerificationToken == command.Token, ct);

        if (user is null || user.EmailVerificationTokenExpiry < DateTime.UtcNow)
            return Error.Custom("InvalidToken", _loc.Get("auth.tokenInvalidOrExpired", lang));

        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiry = null;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
