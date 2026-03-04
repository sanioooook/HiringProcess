using FluentValidation;
using HiringProcess.Api.Common.Localization;

namespace HiringProcess.Api.Features.Auth.Commands;

public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    private readonly ILocalizationService _loc;
    private readonly ICurrentLanguageService _currentLang;

    public LoginValidator(ILocalizationService loc, ICurrentLanguageService currentLang)
    {
        _loc = loc;
        _currentLang = currentLang;

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(_ => _loc.Get("auth.emailRequired", _currentLang.Language))
            .EmailAddress().WithMessage(_ => _loc.Get("auth.emailInvalid", _currentLang.Language));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(_ => _loc.Get("auth.passwordRequired", _currentLang.Language));
    }
}
