namespace HiringProcess.Api.Features.Auth.Models;

/// <summary>
/// Persisted refresh token for silent JWT renewal.
/// Each token can be used exactly once (rotation on every refresh).
/// </summary>
public sealed class RefreshToken
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid UserId { get; init; }

    /// <summary>64-char hex string (32 random bytes).</summary>
    public string Token { get; init; } = string.Empty;

    public DateTime ExpiresAt { get; init; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
