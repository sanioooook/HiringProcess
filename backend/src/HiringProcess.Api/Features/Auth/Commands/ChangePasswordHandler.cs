using FluentValidation;
using HiringProcess.Api.Common;
using HiringProcess.Api.Common.Localization;
using HiringProcess.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HiringProcess.Api.Features.Auth.Commands;

public sealed record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword);

/// <summary>
/// Allows an authenticated user to change their password.
/// Google-only users (no password hash) cannot use this endpoint.
/// </summary>
public sealed class ChangePasswordHandler
{
    private readonly AppDbContext _db;
    private readonly IValidator<ChangePasswordCommand> _validator;
    private readonly ILocalizationService _loc;
    private readonly ICurrentLanguageService _currentLang;

    public ChangePasswordHandler(
        AppDbContext db,
        IValidator<ChangePasswordCommand> validator,
        ILocalizationService loc,
        ICurrentLanguageService currentLang)
    {
        _db = db;
        _validator = validator;
        _loc = loc;
        _currentLang = currentLang;
    }

    public async Task<Result> HandleAsync(ChangePasswordCommand command, CancellationToken ct = default)
    {
        var lang = _currentLang.Language;

        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return Error.Validation(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == command.UserId, ct);
        if (user is null)
            return Error.Custom("NotFound", _loc.Get("settings.userNotFound", lang));

        if (user.PasswordHash is null)
            return Error.Custom("NoPassword", _loc.Get("auth.noPasswordSet", lang));

        if (!BCrypt.Net.BCrypt.Verify(command.CurrentPassword, user.PasswordHash))
            return Error.Custom("InvalidCredentials", _loc.Get("auth.invalidCredentials", lang));

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.NewPassword);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
