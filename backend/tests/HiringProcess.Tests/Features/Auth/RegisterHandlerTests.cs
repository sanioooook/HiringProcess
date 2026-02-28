using HiringProcess.Api.Features.Auth;
using HiringProcess.Api.Features.Auth.Commands;
using HiringProcess.Tests.Common;
using Microsoft.Extensions.Configuration;

namespace HiringProcess.Tests.Features.Auth;

/// <summary>
/// Tests for RegisterHandler.
/// Each test gets its own in-memory SQLite database — no shared state.
/// </summary>
public sealed class RegisterHandlerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly SqliteConnection _connection;
    private readonly RegisterHandler _handler;

    public RegisterHandlerTests()
    {
        (_db, _connection) = TestDbContextFactory.Create();
        var config = BuildConfig();
        var jwt = new JwtService(config);
        var validator = new RegisterValidator();
        _handler = new RegisterHandler(_db, jwt, validator);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var command = new RegisterCommand("user@example.com", "Test User", "Password1");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("user@example.com", result.Value.Email);
        Assert.NotEmpty(result.Value.Token);
    }

    [Fact]
    public async Task HandleAsync_DuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var command = new RegisterCommand("dup@example.com", "User A", "Password1");
        await _handler.HandleAsync(command); // first registration

        // Act
        var result = await _handler.HandleAsync(command); // duplicate

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Conflict", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_EmailIsCaseInsensitive_ReturnsConflict()
    {
        // Arrange
        await _handler.HandleAsync(new RegisterCommand("Case@Example.COM", "User", "Password1"));

        // Act
        var result = await _handler.HandleAsync(new RegisterCommand("case@example.com", "User2", "Password1"));

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Conflict", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_WeakPassword_ReturnsValidationError()
    {
        // Arrange
        var command = new RegisterCommand("user@example.com", "Test", "weak");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Validation", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_InvalidEmail_ReturnsValidationError()
    {
        // Arrange
        var command = new RegisterCommand("not-an-email", "Test", "Password1");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Validation", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_EmptyDisplayName_ReturnsValidationError()
    {
        // Arrange
        var command = new RegisterCommand("user@example.com", "", "Password1");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Validation", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_PersistsUserToDb()
    {
        // Arrange
        var command = new RegisterCommand("persist@example.com", "Persist User", "Password1");

        // Act
        await _handler.HandleAsync(command);

        // Assert
        var user = _db.Users.SingleOrDefault(u => u.Email == "persist@example.com");
        Assert.NotNull(user);
        Assert.Equal("Persist User", user.DisplayName);
        Assert.NotNull(user.PasswordHash);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
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
