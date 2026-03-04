using HiringProcess.Api.Common.Localization;

namespace HiringProcess.Api.Common.Middleware;

public sealed class LanguageMiddleware
{
    private static readonly HashSet<string> _supported = ["en", "uk", "ru"];
    private readonly RequestDelegate _next;

    public LanguageMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ICurrentLanguageService currentLanguage)
    {
        var header = context.Request.Headers["Accept-Language"].FirstOrDefault();
        var lang = header?.Split(',').FirstOrDefault()?.Trim().ToLowerInvariant() ?? "en";
        currentLanguage.Language = _supported.Contains(lang) ? lang : "en";
        await _next(context);
    }
}
