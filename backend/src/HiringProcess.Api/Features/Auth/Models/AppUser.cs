namespace HiringProcess.Api.Features.Auth.Models;

/// <summary>
/// Application user. Supports both password-based and Google OAuth login.
/// </summary>
public sealed class AppUser
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    // Null when the user signed up via Google (no local password)
    public string? PasswordHash { get; set; }

    // Populated when the user authenticated via Google OAuth
    public string? GoogleId { get; set; }

    // Stored as UTC DateTime for SQLite/PostgreSQL compatibility
    public DateTime CreatedAt { get; set; }
}
