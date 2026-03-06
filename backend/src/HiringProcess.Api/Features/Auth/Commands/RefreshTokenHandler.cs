using HiringProcess.Api.Common;
using HiringProcess.Api.Features.Auth.Models;
using HiringProcess.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HiringProcess.Api.Features.Auth.Commands;

/// <summary>
/// Validates a refresh token, revokes it, issues a new access token and a rotated refresh token.
/// Token rotation means each refresh token can only be used once.
/// </summary>
public sealed class RefreshTokenHandler
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;
    private readonly IConfiguration _config;

    public RefreshTokenHandler(AppDbContext db, JwtService jwt, IConfiguration config)
    {
        _db = db;
        _jwt = jwt;
        _config = config;
    }

    public async Task<Result<RefreshTokenResponse>> HandleAsync(
        RefreshTokenCommand command,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
            return Error.Validation("Refresh token is required.");

        // Find the token in the database
        var stored = await _db.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == command.RefreshToken, ct);

        if (stored is null || stored.IsRevoked || stored.ExpiresAt <= DateTime.UtcNow)
            return Error.Custom("InvalidRefreshToken", "Refresh token is invalid or expired.");

        // Find the associated user
        var user = await _db.Users.FindAsync([stored.UserId], ct);
        if (user is null)
            return Error.Custom("InvalidRefreshToken", "Refresh token is invalid or expired.");

        // Revoke the used token (rotation: one-time use)
        stored.IsRevoked = true;

        // Generate new access token + new refresh token
        var newAccessToken = _jwt.GenerateToken(user);
        var newRefreshTokenValue = _jwt.GenerateRefreshToken();
        var expireDays = _config.GetValue<int>("Jwt:RefreshExpireDays", 30);

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(expireDays)
        });

        await _db.SaveChangesAsync(ct);

        return new RefreshTokenResponse(newAccessToken, newRefreshTokenValue);
    }
}
