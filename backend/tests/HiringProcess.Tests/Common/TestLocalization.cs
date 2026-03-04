using HiringProcess.Api.Common.Localization;

namespace HiringProcess.Tests.Common;

internal static class TestLocalization
{
    public static ILocalizationService Loc { get; } = new StubLocalizationService();
    public static ICurrentLanguageService CurrentLang { get; } = new StubCurrentLanguageService();

    private sealed class StubLocalizationService : ILocalizationService
    {
        public string Get(string key, string language) => key;
    }

    private sealed class StubCurrentLanguageService : ICurrentLanguageService
    {
        public string Language { get; set; } = "en";
    }
}
