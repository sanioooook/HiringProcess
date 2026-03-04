using FluentValidation;
using HiringProcess.Api.Common;
using HiringProcess.Api.Common.Localization;
using HiringProcess.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HiringProcess.Api.Features.Auth.Commands;

/// <summary>
/// Handles email + password login.
/// Returns a generic "Invalid credentials" error to prevent user enumeration.
/// </summary>
public sealed class LoginHandler
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;
    private readonly IValidator<LoginCommand> _validator;
    private readonly ILocalizationService _loc;
    private readonly ICurrentLanguageService _currentLang;

    public LoginHandler(
        AppDbContext db,
        JwtService jwt,
        IValidator<LoginCommand> validator,
        ILocalizationService loc,
        ICurrentLanguageService currentLang)
    {
        _db = db;
        _jwt = jwt;
        _validator = validator;
        _loc = loc;
        _currentLang = currentLang;
    }

    public async Task<Result<LoginResponse>> HandleAsync(LoginCommand command, CancellationToken ct = default)
    {
        var lang = _currentLang.Language;

        // Validate input shape
        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            return Error.Validation(message);
        }

        // Look up user by email
        var emailLower = command.Email.ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == emailLower, ct);

        // Verify credentials — same error for missing user or bad password (no enumeration)
        var invalidCreds = Error.Custom("InvalidCredentials", _loc.Get("auth.invalidCredentials", lang));

        if (user is null || user.PasswordHash is null)
            return invalidCreds;

        if (!BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash))
            return invalidCreds;

        // Issue JWT
        var token = _jwt.GenerateToken(user);

        return new LoginResponse(user.Id, user.Email, user.DisplayName, token, user.Language);
    }
}
