using FluentValidation;
using HiringProcess.Api.Common.Localization;

namespace HiringProcess.Api.Features.Auth.Commands;

public sealed class ChangeEmailValidator : AbstractValidator<ChangeEmailCommand>
{
    public ChangeEmailValidator(ILocalizationService loc, ICurrentLanguageService currentLang)
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage(_ => loc.Get("auth.emailRequired", currentLang.Language))
            .EmailAddress().WithMessage(_ => loc.Get("auth.emailInvalid", currentLang.Language))
            .MaximumLength(320);
    }
}
