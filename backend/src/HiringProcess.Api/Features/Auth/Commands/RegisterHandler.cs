using FluentValidation;
using HiringProcess.Api.Common;
using HiringProcess.Api.Features.Auth.Models;
using HiringProcess.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HiringProcess.Api.Features.Auth.Commands;

/// <summary>
/// Handles user registration with email + password.
/// Validates input, checks for duplicate email, hashes password, persists user, returns JWT.
/// </summary>
public sealed class RegisterHandler
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;
    private readonly IValidator<RegisterCommand> _validator;

    public RegisterHandler(AppDbContext db, JwtService jwt, IValidator<RegisterCommand> validator)
    {
        _db = db;
        _jwt = jwt;
        _validator = validator;
    }

    public async Task<Result<RegisterResponse>> HandleAsync(RegisterCommand command, CancellationToken ct = default)
    {
        // Validate input
        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            return Error.Validation(message);
        }

        // Check for duplicate email (case-insensitive)
        var emailLower = command.Email.ToLowerInvariant();
        var exists = await _db.Users.AnyAsync(u => u.Email == emailLower, ct);
        if (exists)
            return Error.Conflict;

        // Hash password using BCrypt-compatible built-in hasher
        var hash = BCrypt.Net.BCrypt.HashPassword(command.Password);

        // Persist new user
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = emailLower,
            DisplayName = command.DisplayName.Trim(),
            PasswordHash = hash,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        // Issue JWT
        var token = _jwt.GenerateToken(user);

        return new RegisterResponse(user.Id, user.Email, user.DisplayName, token);
    }
}
