namespace HiringProcess.Api.Common.Localization;

public sealed class CurrentLanguageService : ICurrentLanguageService
{
    public string Language { get; set; } = "en";
}
