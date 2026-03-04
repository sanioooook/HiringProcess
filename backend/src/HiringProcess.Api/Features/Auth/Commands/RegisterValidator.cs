using FluentValidation;
using HiringProcess.Api.Common.Localization;

namespace HiringProcess.Api.Features.Auth.Commands;

public sealed class RegisterValidator : AbstractValidator<RegisterCommand>
{
    private readonly ILocalizationService _loc;
    private readonly ICurrentLanguageService _currentLang;

    public RegisterValidator(ILocalizationService loc, ICurrentLanguageService currentLang)
    {
        _loc = loc;
        _currentLang = currentLang;

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(_ => _loc.Get("auth.emailRequired", _currentLang.Language))
            .EmailAddress().WithMessage(_ => _loc.Get("auth.emailInvalid", _currentLang.Language))
            .MaximumLength(320);

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage(_ => _loc.Get("auth.displayNameRequired", _currentLang.Language))
            .MaximumLength(200);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(_ => _loc.Get("auth.passwordRequired", _currentLang.Language))
            .MinimumLength(8).WithMessage(_ => _loc.Get("auth.passwordMinLength", _currentLang.Language))
            .MaximumLength(128)
            .Matches("[A-Z]").WithMessage(_ => _loc.Get("auth.passwordUppercase", _currentLang.Language))
            .Matches("[0-9]").WithMessage(_ => _loc.Get("auth.passwordDigit", _currentLang.Language));
    }
}
