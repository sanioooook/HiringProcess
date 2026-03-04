using System.Security.Claims;
using HiringProcess.Api.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiringProcess.Api.Features.UserSettings;

[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UserSettingsController : ControllerBase
{
    private readonly GetUserSettingsHandler _getHandler;
    private readonly UpdateUserSettingsHandler _updateHandler;

    public UserSettingsController(GetUserSettingsHandler getHandler, UpdateUserSettingsHandler updateHandler)
    {
        _getHandler = getHandler;
        _updateHandler = updateHandler;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings(CancellationToken ct)
    {
        var result = await _getHandler.HandleAsync(new GetUserSettingsQuery(CurrentUserId), ct);
        return result.ToActionResult();
    }

    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateSettingsRequest request, CancellationToken ct)
    {
        var command = new UpdateUserSettingsCommand(CurrentUserId, request.Language);
        var result = await _updateHandler.HandleAsync(command, ct);
        return result.ToActionResult();
    }
}

public sealed record UpdateSettingsRequest(string Language);
