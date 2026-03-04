namespace HiringProcess.Api.Common.Localization;

public interface ILocalizationService
{
    string Get(string key, string language);
}
