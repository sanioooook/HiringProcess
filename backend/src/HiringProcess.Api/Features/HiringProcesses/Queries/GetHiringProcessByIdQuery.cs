namespace HiringProcess.Api.Features.HiringProcesses.Queries;

/// <summary>
/// Query for a single hiring process record by its ID.
/// UserId is passed in to enforce data ownership at the handler level.
/// </summary>
public sealed record GetHiringProcessByIdQuery(Guid Id, Guid UserId);
