using FluentValidation;
using HiringProcess.Api.Common;
using HiringProcess.Api.Common.Localization;
using HiringProcess.Api.Infrastructure;
using HiringProcess.Api.Infrastructure.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HiringProcess.Api.Features.Auth.Commands;

public sealed record ChangeEmailCommand(Guid UserId, string NewEmail, string? CurrentPassword);

/// <summary>
/// Initiates an email change: stores the pending new email and sends a confirmation link to it.
/// Password-based users must provide their current password.
/// </summary>
public sealed class ChangeEmailHandler
{
    private readonly AppDbContext _db;
    private readonly IValidator<ChangeEmailCommand> _validator;
    private readonly IEmailService _email;
    private readonly ILocalizationService _loc;
    private readonly ICurrentLanguageService _currentLang;
    private readonly IConfiguration _config;
    private readonly ILogger<ChangeEmailHandler> _logger;

    public ChangeEmailHandler(
        AppDbContext db,
        IValidator<ChangeEmailCommand> validator,
        IEmailService email,
        ILocalizationService loc,
        ICurrentLanguageService currentLang,
        IConfiguration config,
        ILogger<ChangeEmailHandler> logger)
    {
        _db = db;
        _validator = validator;
        _email = email;
        _loc = loc;
        _currentLang = currentLang;
        _config = config;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(ChangeEmailCommand command, CancellationToken ct = default)
    {
        var lang = _currentLang.Language;

        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return Error.Validation(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == command.UserId, ct);
        if (user is null)
            return Error.Custom("NotFound", _loc.Get("settings.userNotFound", lang));

        // Password-based users must verify their current password
        if (user.PasswordHash is not null)
        {
            if (string.IsNullOrWhiteSpace(command.CurrentPassword) ||
                !BCrypt.Net.BCrypt.Verify(command.CurrentPassword, user.PasswordHash))
                return Error.Custom("InvalidCredentials", _loc.Get("auth.invalidCredentials", lang));
        }

        var newEmailLower = command.NewEmail.ToLowerInvariant();

        // Ensure new email is not already taken by another user
        var taken = await _db.Users.AnyAsync(u => u.Email == newEmailLower && u.Id != command.UserId, ct);
        if (taken)
            return Error.Custom("Conflict", _loc.Get("auth.emailTaken", lang));

        user.PendingEmail = newEmailLower;
        user.EmailChangeToken = RegisterHandler.GenerateToken();
        user.EmailChangeTokenExpiry = DateTime.UtcNow.AddHours(24);
        await _db.SaveChangesAsync(ct);

        try
        {
            var frontendUrl = _config["App:FrontendUrl"] ?? "http://localhost:4200";
            var link = $"{frontendUrl}/confirm-email-change?token={user.EmailChangeToken}";
            var subject = _loc.Get("email.changeEmailSubject", lang);
            var body = $"<p>{_loc.Get("email.changeEmailBody", lang)}</p><p><a href=\"{link}\">{link}</a></p>";
            await _email.SendAsync(newEmailLower, subject, body, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email change confirmation to {Email}", newEmailLower);
        }

        return Result.Success();
    }
}
