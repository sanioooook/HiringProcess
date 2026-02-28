namespace HiringProcess.Api.Features.HiringProcesses.Commands;

/// <summary>
/// Command to permanently delete a hiring process owned by the given user.
/// </summary>
public sealed record DeleteHiringProcessCommand(Guid Id, Guid UserId);
