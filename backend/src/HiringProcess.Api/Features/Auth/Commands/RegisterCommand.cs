namespace HiringProcess.Api.Features.Auth.Commands;

/// <summary>
/// Data required to register a new user with email + password.
/// </summary>
public sealed record RegisterCommand(
    string Email,
    string DisplayName,
    string Password
);

/// <summary>
/// Response returned after successful registration.
/// </summary>
public sealed record RegisterResponse(
    Guid UserId,
    string Email,
    string DisplayName,
    string Token,
    string Language,
    string RefreshToken
);
