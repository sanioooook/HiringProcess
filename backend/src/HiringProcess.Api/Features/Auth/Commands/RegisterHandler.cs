using FluentValidation;
using HiringProcess.Api.Common;
using HiringProcess.Api.Common.Localization;
using HiringProcess.Api.Features.Auth.Models;
using HiringProcess.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HiringProcess.Api.Features.Auth.Commands;

/// <summary>
/// Handles user registration with email + password.
/// Validates input, checks for duplicate email, hashes password, persists user, returns JWT.
/// </summary>
public sealed class RegisterHandler
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;
    private readonly IValidator<RegisterCommand> _validator;
    private readonly ILocalizationService _loc;
    private readonly ICurrentLanguageService _currentLang;
    private readonly IConfiguration _config;

    public RegisterHandler(
        AppDbContext db,
        JwtService jwt,
        IValidator<RegisterCommand> validator,
        ILocalizationService loc,
        ICurrentLanguageService currentLang,
        IConfiguration config)
    {
        _db = db;
        _jwt = jwt;
        _validator = validator;
        _loc = loc;
        _currentLang = currentLang;
        _config = config;
    }

    public async Task<Result<RegisterResponse>> HandleAsync(RegisterCommand command, CancellationToken ct = default)
    {
        var lang = _currentLang.Language;

        // Validate input
        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            return Error.Validation(message);
        }

        // Check for duplicate email (case-insensitive)
        var emailLower = command.Email.ToLowerInvariant();
        var exists = await _db.Users.AnyAsync(u => u.Email == emailLower, ct);
        if (exists)
            return Error.Custom("Conflict", _loc.Get("auth.emailTaken", lang));

        // Hash password
        var hash = BCrypt.Net.BCrypt.HashPassword(command.Password);

        // Persist new user with the current language preference
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = emailLower,
            DisplayName = command.DisplayName.Trim(),
            PasswordHash = hash,
            Language = lang,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        // Issue JWT + refresh token
        var token = _jwt.GenerateToken(user);
        var refreshTokenValue = _jwt.GenerateRefreshToken();
        var expireDays = _config.GetValue<int>("Jwt:RefreshExpireDays", 30);

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(expireDays)
        });
        await _db.SaveChangesAsync(ct);

        return new RegisterResponse(user.Id, user.Email, user.DisplayName, token, user.Language, refreshTokenValue);
    }
}
