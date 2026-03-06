namespace HiringProcess.Api.Features.Auth.Commands;

/// <summary>Data required to exchange a refresh token for a new access token.</summary>
public sealed record RefreshTokenCommand(string RefreshToken);

/// <summary>New access token + rotated refresh token.</summary>
public sealed record RefreshTokenResponse(
    string Token,
    string RefreshToken
);
