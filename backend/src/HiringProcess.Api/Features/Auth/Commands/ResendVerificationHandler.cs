using HiringProcess.Api.Common;
using HiringProcess.Api.Common.Localization;
using HiringProcess.Api.Infrastructure;
using HiringProcess.Api.Infrastructure.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HiringProcess.Api.Features.Auth.Commands;

public sealed record ResendVerificationCommand(string Email);

/// <summary>
/// Regenerates and resends the email verification link.
/// Always returns success to prevent user enumeration.
/// </summary>
public sealed class ResendVerificationHandler
{
    private readonly AppDbContext _db;
    private readonly IEmailService _email;
    private readonly ILocalizationService _loc;
    private readonly ICurrentLanguageService _currentLang;
    private readonly IConfiguration _config;
    private readonly ILogger<ResendVerificationHandler> _logger;

    public ResendVerificationHandler(
        AppDbContext db,
        IEmailService email,
        ILocalizationService loc,
        ICurrentLanguageService currentLang,
        IConfiguration config,
        ILogger<ResendVerificationHandler> logger)
    {
        _db = db;
        _email = email;
        _loc = loc;
        _currentLang = currentLang;
        _config = config;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(ResendVerificationCommand command, CancellationToken ct = default)
    {
        var lang = _currentLang.Language;
        var emailLower = command.Email.ToLowerInvariant();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == emailLower, ct);

        // Silent success if user not found or already verified (no enumeration)
        if (user is null || user.IsEmailVerified)
            return Result.Success();

        // Regenerate token
        user.EmailVerificationToken = RegisterHandler.GenerateToken();
        user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24);
        await _db.SaveChangesAsync(ct);

        try
        {
            var frontendUrl = _config["App:FrontendUrl"] ?? "http://localhost:4200";
            var link = $"{frontendUrl}/verify-email?token={user.EmailVerificationToken}";
            var subject = _loc.Get("email.verifySubject", lang);
            var body = $"<p>{_loc.Get("email.verifyBody", lang)}</p><p><a href=\"{link}\">{link}</a></p>";
            await _email.SendAsync(user.Email, subject, body, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resend verification email to {Email}", user.Email);
        }

        return Result.Success();
    }
}
