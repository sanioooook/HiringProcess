namespace HiringProcess.Api.Features.UserSettings;

public sealed record UpdateUserSettingsCommand(Guid UserId, string Language);
