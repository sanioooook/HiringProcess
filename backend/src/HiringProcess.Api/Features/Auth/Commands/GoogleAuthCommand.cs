namespace HiringProcess.Api.Features.Auth.Commands;

/// <summary>
/// Data received after Google verifies the user on the frontend.
/// The frontend exchanges the Google auth code for an ID token and passes it here.
/// </summary>
public sealed record GoogleAuthCommand(string IdToken);

/// <summary>
/// Response for both Google sign-in and sign-up (same handler, upsert logic).
/// </summary>
public sealed record GoogleAuthResponse(
    Guid UserId,
    string Email,
    string DisplayName,
    string Token,
    bool IsNewUser,
    string Language,
    string RefreshToken
);
