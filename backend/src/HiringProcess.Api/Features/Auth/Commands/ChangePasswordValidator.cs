using FluentValidation;
using HiringProcess.Api.Common.Localization;

namespace HiringProcess.Api.Features.Auth.Commands;

public sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator(ILocalizationService loc, ICurrentLanguageService currentLang)
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage(_ => loc.Get("auth.passwordRequired", currentLang.Language));

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage(_ => loc.Get("auth.passwordRequired", currentLang.Language))
            .MinimumLength(8).WithMessage(_ => loc.Get("auth.passwordMinLength", currentLang.Language))
            .MaximumLength(128)
            .Matches("[A-Z]").WithMessage(_ => loc.Get("auth.passwordUppercase", currentLang.Language))
            .Matches("[0-9]").WithMessage(_ => loc.Get("auth.passwordDigit", currentLang.Language));
    }
}
