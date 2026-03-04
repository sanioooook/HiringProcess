using FluentValidation;
using HiringProcess.Api.Common.Localization;

namespace HiringProcess.Api.Features.HiringProcesses.Commands;

public sealed class CreateHiringProcessValidator : AbstractValidator<CreateHiringProcessCommand>
{
    private readonly ILocalizationService _loc;
    private readonly ICurrentLanguageService _currentLang;

    public CreateHiringProcessValidator(ILocalizationService loc, ICurrentLanguageService currentLang)
    {
        _loc = loc;
        _currentLang = currentLang;

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage(_ => _loc.Get("hp.companyRequired", _currentLang.Language))
            .MaximumLength(500);

        RuleFor(x => x.ContactChannel)
            .NotEmpty().WithMessage(_ => _loc.Get("hp.channelRequired", _currentLang.Language))
            .MaximumLength(200);

        RuleFor(x => x.ContactPerson)
            .MaximumLength(300).When(x => x.ContactPerson is not null);

        RuleFor(x => x.AppliedLink)
            .Must(BeAValidUrlOrNull).WithMessage(_ => _loc.Get("hp.appliedLinkUrl", _currentLang.Language))
            .When(x => !string.IsNullOrWhiteSpace(x.AppliedLink));

        RuleFor(x => x.VacancyLink)
            .Must(BeAValidUrlOrNull).WithMessage(_ => _loc.Get("hp.vacancyLinkUrl", _currentLang.Language))
            .When(x => !string.IsNullOrWhiteSpace(x.VacancyLink));

        RuleFor(x => x.SalaryRange)
            .MaximumLength(200).When(x => x.SalaryRange is not null);

        RuleFor(x => x.CurrentStage)
            .MaximumLength(200).When(x => x.CurrentStage is not null);

        RuleFor(x => x.HiringStages)
            .Must(stages => stages == null || stages.All(s => s.Length <= 200))
            .WithMessage(_ => _loc.Get("hp.stageTooLong", _currentLang.Language));
    }

    private static bool BeAValidUrlOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return true;
        return Uri.TryCreate(value, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
