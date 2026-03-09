using FluentValidation;
using HiringProcess.Api.Common;
using HiringProcess.Api.Common.Localization;
using HiringProcess.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HiringProcess.Api.Features.Auth.Commands;

public sealed record ResetPasswordCommand(string Token, string NewPassword);

/// <summary>
/// Sets a new password using a valid (non-expired) reset token.
/// </summary>
public sealed class ResetPasswordHandler
{
    private readonly AppDbContext _db;
    private readonly IValidator<ResetPasswordCommand> _validator;
    private readonly ILocalizationService _loc;
    private readonly ICurrentLanguageService _currentLang;

    public ResetPasswordHandler(
        AppDbContext db,
        IValidator<ResetPasswordCommand> validator,
        ILocalizationService loc,
        ICurrentLanguageService currentLang)
    {
        _db = db;
        _validator = validator;
        _loc = loc;
        _currentLang = currentLang;
    }

    public async Task<Result> HandleAsync(ResetPasswordCommand command, CancellationToken ct = default)
    {
        var lang = _currentLang.Language;

        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return Error.Validation(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.PasswordResetToken == command.Token, ct);

        if (user is null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
            return Error.Custom("InvalidToken", _loc.Get("auth.tokenInvalidOrExpired", lang));

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
