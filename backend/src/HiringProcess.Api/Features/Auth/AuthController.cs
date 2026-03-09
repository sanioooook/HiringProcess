using System.Security.Claims;
using HiringProcess.Api.Common.Extensions;
using HiringProcess.Api.Features.Auth.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiringProcess.Api.Features.Auth;

/// <summary>
/// HTTP adapter for authentication operations.
/// Controller only performs HTTP mapping - all business logic is in handlers.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly RegisterHandler _registerHandler;
    private readonly LoginHandler _loginHandler;
    private readonly GoogleAuthHandler _googleAuthHandler;
    private readonly RefreshTokenHandler _refreshTokenHandler;
    private readonly VerifyEmailHandler _verifyEmailHandler;
    private readonly ResendVerificationHandler _resendVerificationHandler;
    private readonly ForgotPasswordHandler _forgotPasswordHandler;
    private readonly ResetPasswordHandler _resetPasswordHandler;
    private readonly ChangePasswordHandler _changePasswordHandler;
    private readonly ChangeEmailHandler _changeEmailHandler;
    private readonly ConfirmEmailChangeHandler _confirmEmailChangeHandler;

    public AuthController(
        RegisterHandler registerHandler,
        LoginHandler loginHandler,
        GoogleAuthHandler googleAuthHandler,
        RefreshTokenHandler refreshTokenHandler,
        VerifyEmailHandler verifyEmailHandler,
        ResendVerificationHandler resendVerificationHandler,
        ForgotPasswordHandler forgotPasswordHandler,
        ResetPasswordHandler resetPasswordHandler,
        ChangePasswordHandler changePasswordHandler,
        ChangeEmailHandler changeEmailHandler,
        ConfirmEmailChangeHandler confirmEmailChangeHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
        _googleAuthHandler = googleAuthHandler;
        _refreshTokenHandler = refreshTokenHandler;
        _verifyEmailHandler = verifyEmailHandler;
        _resendVerificationHandler = resendVerificationHandler;
        _forgotPasswordHandler = forgotPasswordHandler;
        _resetPasswordHandler = resetPasswordHandler;
        _changePasswordHandler = changePasswordHandler;
        _changeEmailHandler = changeEmailHandler;
        _confirmEmailChangeHandler = confirmEmailChangeHandler;
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

    /// <summary>Exchange a refresh token for a new access token (token rotation).</summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenCommand command,
        CancellationToken ct)
    {
        var result = await _refreshTokenHandler.HandleAsync(command, ct);
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

    /// <summary>Verify email using the token sent during registration.</summary>
    [HttpPost("verify-email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command, CancellationToken ct)
    {
        var result = await _verifyEmailHandler.HandleAsync(command, ct);
        return result.IsSuccess ? NoContent() : result.ToActionResult();
    }

    /// <summary>Resend the email verification link.</summary>
    [HttpPost("resend-verification")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationCommand command, CancellationToken ct)
    {
        await _resendVerificationHandler.HandleAsync(command, ct);
        return NoContent();
    }

    /// <summary>Send a password reset link to the given email address.</summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken ct)
    {
        await _forgotPasswordHandler.HandleAsync(command, ct);
        return NoContent();
    }

    /// <summary>Reset the password using a valid reset token.</summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken ct)
    {
        var result = await _resetPasswordHandler.HandleAsync(command, ct);
        return result.IsSuccess ? NoContent() : result.ToActionResult();
    }

    /// <summary>Change the current user's password. Requires authentication.</summary>
    [HttpPut("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var command = new ChangePasswordCommand(userId.Value, request.CurrentPassword, request.NewPassword);
        var result = await _changePasswordHandler.HandleAsync(command, ct);
        return result.IsSuccess ? NoContent() : result.ToActionResult();
    }

    /// <summary>Initiate an email change. Sends confirmation link to the new address. Requires authentication.</summary>
    [HttpPut("change-email")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var command = new ChangeEmailCommand(userId.Value, request.NewEmail, request.CurrentPassword);
        var result = await _changeEmailHandler.HandleAsync(command, ct);
        return result.IsSuccess ? NoContent() : result.ToActionResult();
    }

    /// <summary>Confirm email change using the token sent to the new address.</summary>
    [HttpPost("confirm-email-change")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmailChange([FromBody] ConfirmEmailChangeCommand command, CancellationToken ct)
    {
        var result = await _confirmEmailChangeHandler.HandleAsync(command, ct);
        return result.IsSuccess ? NoContent() : result.ToActionResult();
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}

// Request DTOs (body fields differ from command constructor due to UserId coming from JWT)
public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public sealed record ChangeEmailRequest(string NewEmail, string? CurrentPassword);
