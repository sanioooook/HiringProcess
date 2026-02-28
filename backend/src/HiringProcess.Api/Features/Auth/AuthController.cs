using HiringProcess.Api.Common.Extensions;
using HiringProcess.Api.Features.Auth.Commands;
using Microsoft.AspNetCore.Mvc;

namespace HiringProcess.Api.Features.Auth;

/// <summary>
/// HTTP adapter for authentication operations.
/// Controller only performs HTTP mapping — all business logic is in handlers.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly RegisterHandler _registerHandler;
    private readonly LoginHandler _loginHandler;
    private readonly GoogleAuthHandler _googleAuthHandler;

    public AuthController(
        RegisterHandler registerHandler,
        LoginHandler loginHandler,
        GoogleAuthHandler googleAuthHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
        _googleAuthHandler = googleAuthHandler;
    }

    /// <summary>Register a new user with email and password.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        CancellationToken ct)
    {
        var result = await _registerHandler.HandleAsync(command, ct);

        if (result.IsFailure)
            return result.ToActionResult();

        return CreatedAtAction(nameof(Register), result.Value);
    }

    /// <summary>Login with email and password.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken ct)
    {
        var result = await _loginHandler.HandleAsync(command, ct);
        return result.ToActionResult();
    }

    /// <summary>Authenticate or register via Google ID token.</summary>
    [HttpPost("google")]
    [ProducesResponseType(typeof(GoogleAuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Google(
        [FromBody] GoogleAuthCommand command,
        CancellationToken ct)
    {
        var result = await _googleAuthHandler.HandleAsync(command, ct);
        return result.ToActionResult();
    }
}
