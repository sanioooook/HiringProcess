using FluentValidation;
using HiringProcess.Api.Common.Localization;

namespace HiringProcess.Api.Features.UserSettings;

public sealed class UpdateUserSettingsValidator : AbstractValidator<UpdateUserSettingsCommand>
{
    private static readonly string[] _supported = ["en", "uk", "ru"];

    private readonly ILocalizationService _loc;
    private readonly ICurrentLanguageService _currentLang;

    public UpdateUserSettingsValidator(ILocalizationService loc, ICurrentLanguageService currentLang)
    {
        _loc = loc;
        _currentLang = currentLang;

        RuleFor(x => x.Language)
            .NotEmpty()
            .Must(lang => _supported.Contains(lang))
            .WithMessage(_ => _loc.Get("settings.invalidLanguage", _currentLang.Language));
    }
}
