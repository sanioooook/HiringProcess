using Google.Apis.Auth;
using HiringProcess.Api.Common;
using HiringProcess.Api.Features.Auth.Models;
using HiringProcess.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HiringProcess.Api.Features.Auth.Commands;

/// <summary>
/// Validates a Google ID token, then upserts the user (create on first sign-in, update on subsequent).
/// Returns a JWT regardless of whether this was a registration or login.
/// </summary>
public sealed class GoogleAuthHandler
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;
    private readonly IConfiguration _config;

    public GoogleAuthHandler(AppDbContext db, JwtService jwt, IConfiguration config)
    {
        _db = db;
        _jwt = jwt;
        _config = config;
    }

    public async Task<Result<GoogleAuthResponse>> HandleAsync(GoogleAuthCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.IdToken))
            return Error.Validation("Google ID token is required.");

        // Verify the Google ID token
        GoogleJsonWebSignature.Payload payload;
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_config["Authentication:Google:ClientId"]
                    ?? throw new InvalidOperationException("Google ClientId is not configured.")]
            };
            payload = await GoogleJsonWebSignature.ValidateAsync(command.IdToken, settings);
        }
        catch (InvalidJwtException ex)
        {
            return Error.Custom("InvalidGoogleToken", $"Google token validation failed: {ex.Message}");
        }

        var googleId = payload.Subject;
        var email = payload.Email.ToLowerInvariant();
        var displayName = payload.Name ?? email;

        // Try to find an existing user by Google ID, then fall back to email
        var user = await _db.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId, ct)
                   ?? await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

        bool isNewUser = false;

        if (user is null)
        {
            // First-time Google sign-in — create account
            user = new AppUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                DisplayName = displayName,
                GoogleId = googleId,
                CreatedAt = DateTime.UtcNow
            };
            _db.Users.Add(user);
            isNewUser = true;
        }
        else
        {
            // Returning user — make sure Google ID is linked
            if (user.GoogleId is null)
                user.GoogleId = googleId;
        }

        await _db.SaveChangesAsync(ct);

        // Issue JWT
        var token = _jwt.GenerateToken(user);

        return new GoogleAuthResponse(user.Id, user.Email, user.DisplayName, token, isNewUser);
    }
}
