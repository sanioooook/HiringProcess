using HiringProcess.Api.Features.Auth;
using HiringProcess.Api.Features.Auth.Commands;
using HiringProcess.Api.Features.Auth.Models;
using HiringProcess.Tests.Common;
using Microsoft.Extensions.Configuration;

namespace HiringProcess.Tests.Features.Auth;

/// <summary>
/// Tests for LoginHandler. Each test uses an isolated in-memory database.
/// </summary>
public sealed class LoginHandlerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly SqliteConnection _connection;
    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        (_db, _connection) = TestDbContextFactory.Create();
        var config = BuildConfig();
        var jwt = new JwtService(config);
        var validator = new LoginValidator(TestLocalization.Loc, TestLocalization.CurrentLang);
        _handler = new LoginHandler(_db, jwt, validator, TestLocalization.Loc, TestLocalization.CurrentLang);
    }

    [Fact]
    public async Task HandleAsync_ValidCredentials_ReturnsToken()
    {
        // Arrange - seed a user
        await SeedUserAsync("login@example.com", "CorrectPass1");

        var command = new LoginCommand("login@example.com", "CorrectPass1");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value!.Token);
    }

    [Fact]
    public async Task HandleAsync_WrongPassword_ReturnsInvalidCredentials()
    {
        // Arrange
        await SeedUserAsync("user2@example.com", "CorrectPass1");

        var command = new LoginCommand("user2@example.com", "WrongPassword");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("InvalidCredentials", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_UnknownEmail_ReturnsInvalidCredentials()
    {
        // Act
        var result = await _handler.HandleAsync(new LoginCommand("nobody@example.com", "Pass1"));

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("InvalidCredentials", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_EmptyEmail_ReturnsValidationError()
    {
        var result = await _handler.HandleAsync(new LoginCommand("", "Pass1"));
        Assert.True(result.IsFailure);
        Assert.Equal("Validation", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_EmptyPassword_ReturnsValidationError()
    {
        var result = await _handler.HandleAsync(new LoginCommand("user@example.com", ""));
        Assert.True(result.IsFailure);
        Assert.Equal("Validation", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_LoginIsCaseInsensitive_ReturnsSuccess()
    {
        // Arrange
        await SeedUserAsync("CaseTest@Example.com", "Password1");

        // Act - lowercase version of the email
        var result = await _handler.HandleAsync(new LoginCommand("casetest@example.com", "Password1"));

        // Assert
        Assert.True(result.IsSuccess);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    private async Task SeedUserAsync(string email, string password)
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant(),
            DisplayName = "Test User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    private static IConfiguration BuildConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "super-secret-key-for-testing-only-32chars!!",
                ["Jwt:Issuer"] = "test",
                ["Jwt:Audience"] = "test",
                ["Jwt:ExpireMinutes"] = "60"
            })
            .Build();
}
