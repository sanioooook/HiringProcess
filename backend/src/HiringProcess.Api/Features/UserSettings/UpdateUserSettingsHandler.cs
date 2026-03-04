using FluentValidation;
using HiringProcess.Api.Common;
using HiringProcess.Api.Common.Localization;
using HiringProcess.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HiringProcess.Api.Features.UserSettings;

public sealed class UpdateUserSettingsHandler
{
    private readonly AppDbContext _db;
    private readonly IValidator<UpdateUserSettingsCommand> _validator;
    private readonly ILocalizationService _loc;
    private readonly ICurrentLanguageService _currentLang;

    public UpdateUserSettingsHandler(
        AppDbContext db,
        IValidator<UpdateUserSettingsCommand> validator,
        ILocalizationService loc,
        ICurrentLanguageService currentLang)
    {
        _db = db;
        _validator = validator;
        _loc = loc;
        _currentLang = currentLang;
    }

    public async Task<Result<UserSettingsResponse>> HandleAsync(UpdateUserSettingsCommand command, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            return Error.Validation(message);
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == command.UserId, ct);
        if (user is null)
            return Error.Custom("NotFound", _loc.Get("settings.userNotFound", _currentLang.Language));

        user.Language = command.Language;
        await _db.SaveChangesAsync(ct);

        return new UserSettingsResponse(user.Language);
    }
}
