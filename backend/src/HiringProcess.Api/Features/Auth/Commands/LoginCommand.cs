namespace HiringProcess.Api.Features.Auth.Commands;

/// <summary>
/// Data required to authenticate with email + password.
/// </summary>
public sealed record LoginCommand(string Email, string Password);

/// <summary>
/// Response returned after successful login.
/// </summary>
public sealed record LoginResponse(
    Guid UserId,
    string Email,
    string DisplayName,
    string Token,
    string Language,
    string RefreshToken,
    bool HasPassword
);
