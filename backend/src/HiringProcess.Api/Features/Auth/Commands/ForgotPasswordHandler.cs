using HiringProcess.Api.Common;
using HiringProcess.Api.Common.Localization;
using HiringProcess.Api.Infrastructure;
using HiringProcess.Api.Infrastructure.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HiringProcess.Api.Features.Auth.Commands;

public sealed record ForgotPasswordCommand(string Email);

/// <summary>
/// Generates a password reset token and sends a reset link by email.
/// Always returns success to prevent user enumeration.
/// </summary>
public sealed class ForgotPasswordHandler
{
    private readonly AppDbContext _db;
    private readonly IEmailService _email;
    private readonly ILocalizationService _loc;
    private readonly ICurrentLanguageService _currentLang;
    private readonly IConfiguration _config;
    private readonly ILogger<ForgotPasswordHandler> _logger;

    public ForgotPasswordHandler(
        AppDbContext db,
        IEmailService email,
        ILocalizationService loc,
        ICurrentLanguageService currentLang,
        IConfiguration config,
        ILogger<ForgotPasswordHandler> logger)
    {
        _db = db;
        _email = email;
        _loc = loc;
        _currentLang = currentLang;
        _config = config;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(ForgotPasswordCommand command, CancellationToken ct = default)
    {
        var lang = _currentLang.Language;
        var emailLower = command.Email.ToLowerInvariant();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == emailLower, ct);

        // Silent success if user not found or is a Google-only user (no password)
        if (user is null || user.PasswordHash is null)
            return Result.Success();

        user.PasswordResetToken = RegisterHandler.GenerateToken();
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        await _db.SaveChangesAsync(ct);

        try
        {
            var frontendUrl = _config["App:FrontendUrl"] ?? "http://localhost:4200";
            var link = $"{frontendUrl}/reset-password?token={user.PasswordResetToken}";
            var subject = _loc.Get("email.resetSubject", lang);
            var body = $"<p>{_loc.Get("email.resetBody", lang)}</p><p><a href=\"{link}\">{link}</a></p>";
            await _email.SendAsync(user.Email, subject, body, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
        }

        return Result.Success();
    }
}
