namespace HiringProcess.Api.Features.UserSettings;

public sealed record GetUserSettingsQuery(Guid UserId);

public sealed record UserSettingsResponse(string Language);
