using HiringProcess.Api.Features.HiringProcesses.Commands;
using HiringProcess.Tests.Common;

namespace HiringProcess.Tests.Features.HiringProcesses;

/// <summary>
/// Tests for CreateHiringProcessHandler. Each test uses an isolated in-memory database.
/// </summary>
public sealed class CreateHiringProcessHandlerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly SqliteConnection _connection;
    private readonly CreateHiringProcessHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();

    public CreateHiringProcessHandlerTests()
    {
        (_db, _connection) = TestDbContextFactory.Create();
        _handler = new CreateHiringProcessHandler(_db, new CreateHiringProcessValidator());
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_ReturnsDtoWithId()
    {
        // Arrange
        var command = BuildCommand();

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value!.Id);
        Assert.Equal("Acme Corp", result.Value.CompanyName);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_PersistsToDb()
    {
        // Arrange
        var command = BuildCommand();

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        var entity = _db.HiringProcesses.Find(result.Value!.Id);
        Assert.NotNull(entity);
        Assert.Equal(_userId, entity.UserId);
    }

    [Fact]
    public async Task HandleAsync_MissingCompanyName_ReturnsValidationError()
    {
        // Arrange
        var command = BuildCommand() with { CompanyName = "" };

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Validation", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_MissingContactChannel_ReturnsValidationError()
    {
        // Arrange
        var command = BuildCommand() with { ContactChannel = "" };

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Validation", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_InvalidAppliedLink_ReturnsValidationError()
    {
        // Arrange
        var command = BuildCommand() with { AppliedLink = "not-a-url" };

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Validation", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_WithHiringStages_StoresAndReturnsStages()
    {
        // Arrange
        var stages = new List<string> { "Applied", "Phone Screen", "Technical" };
        var command = BuildCommand() with { HiringStages = stages, CurrentStage = "Phone Screen" };

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value!.HiringStages.Count);
        Assert.Contains("Phone Screen", result.Value.HiringStages);
        Assert.Equal("Phone Screen", result.Value.CurrentStage);
    }

    [Fact]
    public async Task HandleAsync_SetsCreatedAtAndUpdatedAt()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);
        var command = BuildCommand();

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        var after = DateTime.UtcNow.AddSeconds(1);
        Assert.True(result.IsSuccess);
        Assert.InRange(result.Value!.CreatedAt, before, after);
        Assert.InRange(result.Value.UpdatedAt, before, after);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    private CreateHiringProcessCommand BuildCommand() => new(
        UserId: _userId,
        CompanyName: "Acme Corp",
        ContactChannel: "LinkedIn",
        ContactPerson: "Jane Smith",
        FirstContactDate: new DateOnly(2024, 1, 15),
        LastContactDate: null,
        VacancyPublishedDate: null,
        ApplicationDate: new DateOnly(2024, 1, 20),
        AppliedWith: "Resume",
        AppliedLink: null,
        CoverLetter: null,
        SalaryRange: "80k-100k",
        HiringStages: null,
        CurrentStage: null,
        VacancyLink: null,
        VacancyText: null,
        Notes: null
    );
}
