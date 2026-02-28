using FluentValidation;

namespace HiringProcess.Api.Features.HiringProcesses.Commands;

public sealed class UpdateHiringProcessValidator : AbstractValidator<UpdateHiringProcessCommand>
{
    public UpdateHiringProcessValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(500);

        RuleFor(x => x.ContactChannel)
            .NotEmpty().WithMessage("Contact channel is required.")
            .MaximumLength(200);

        RuleFor(x => x.ContactPerson)
            .MaximumLength(300).When(x => x.ContactPerson is not null);

        RuleFor(x => x.AppliedLink)
            .Must(BeAValidUrlOrNull).WithMessage("Applied link must be a valid URL.")
            .When(x => !string.IsNullOrWhiteSpace(x.AppliedLink));

        RuleFor(x => x.VacancyLink)
            .Must(BeAValidUrlOrNull).WithMessage("Vacancy link must be a valid URL.")
            .When(x => !string.IsNullOrWhiteSpace(x.VacancyLink));

        RuleFor(x => x.SalaryRange)
            .MaximumLength(200).When(x => x.SalaryRange is not null);

        RuleFor(x => x.CurrentStage)
            .MaximumLength(200).When(x => x.CurrentStage is not null);

        RuleFor(x => x.HiringStages)
            .Must(stages => stages == null || stages.All(s => s.Length <= 200))
            .WithMessage("Each hiring stage must be at most 200 characters.");
    }

    private static bool BeAValidUrlOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return true;
        return Uri.TryCreate(value, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
