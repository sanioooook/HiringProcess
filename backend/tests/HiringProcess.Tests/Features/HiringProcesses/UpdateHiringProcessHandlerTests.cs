using HiringProcess.Api.Features.HiringProcesses.Commands;
using HiringProcess.Api.Features.HiringProcesses.Models;
using HiringProcess.Tests.Common;

namespace HiringProcess.Tests.Features.HiringProcesses;

/// <summary>
/// Tests for UpdateHiringProcessHandler. Each test uses an isolated in-memory database.
/// </summary>
public sealed class UpdateHiringProcessHandlerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly SqliteConnection _connection;
    private readonly UpdateHiringProcessHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();

    public UpdateHiringProcessHandlerTests()
    {
        (_db, _connection) = TestDbContextFactory.Create();
        _handler = new UpdateHiringProcessHandler(_db, new UpdateHiringProcessValidator(TestLocalization.Loc, TestLocalization.CurrentLang));
    }

    [Fact]
    public async Task HandleAsync_ExistingRecord_UpdatesAndReturnsDto()
    {
        // Arrange
        var entity = await SeedEntityAsync();
        var command = BuildCommand(entity.Id) with { CompanyName = "Updated Corp" };

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Corp", result.Value!.CompanyName);
    }

    [Fact]
    public async Task HandleAsync_WrongUserId_ReturnsNotFound()
    {
        // Arrange
        var entity = await SeedEntityAsync();
        var command = BuildCommand(entity.Id) with { UserId = Guid.NewGuid() };

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NotFound", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_NonExistentId_ReturnsNotFound()
    {
        // Act
        var result = await _handler.HandleAsync(BuildCommand(Guid.NewGuid()));

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NotFound", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_EmptyCompanyName_ReturnsValidationError()
    {
        // Arrange
        var entity = await SeedEntityAsync();
        var command = BuildCommand(entity.Id) with { CompanyName = "" };

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Validation", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_UpdatesUpdatedAtTimestamp()
    {
        // Arrange
        var entity = await SeedEntityAsync();
        var before = entity.UpdatedAt;
        await Task.Delay(10); // ensure time progresses

        // Act
        var result = await _handler.HandleAsync(BuildCommand(entity.Id));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.UpdatedAt > before);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    private async Task<HiringProcessEntity> SeedEntityAsync()
    {
        var entity = new HiringProcessEntity
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            CompanyName = "Original Corp",
            ContactChannel = "Email",
            HiringStagesRaw = string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _db.HiringProcesses.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    private UpdateHiringProcessCommand BuildCommand(Guid id) => new(
        Id: id,
        UserId: _userId,
        CompanyName: "Updated Corp",
        ContactChannel: "LinkedIn",
        ContactPerson: null,
        FirstContactDate: null,
        LastContactDate: null,
        VacancyPublishedDate: null,
        ApplicationDate: null,
        AppliedWith: null,
        AppliedLink: null,
        CoverLetter: null,
        SalaryRange: null,
        HiringStages: null,
        CurrentStage: null,
        VacancyLink: null,
        VacancyText: null,
        Notes: null
    );
}
