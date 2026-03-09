using HiringProcess.Api.Features.HiringProcesses.Models;
using HiringProcess.Api.Features.HiringProcesses.Queries;
using HiringProcess.Tests.Common;

namespace HiringProcess.Tests.Features.HiringProcesses;

/// <summary>
/// Tests for GetHiringProcessByIdHandler - ownership enforcement and not-found scenarios.
/// </summary>
public sealed class GetHiringProcessByIdHandlerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly SqliteConnection _connection;
    private readonly GetHiringProcessByIdHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();

    public GetHiringProcessByIdHandlerTests()
    {
        (_db, _connection) = TestDbContextFactory.Create();
        _handler = new GetHiringProcessByIdHandler(_db);
    }

    [Fact]
    public async Task HandleAsync_OwnedRecord_ReturnsDto()
    {
        // Arrange
        var entity = await SeedAsync();
        var query = new GetHiringProcessByIdQuery(entity.Id, _userId);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(entity.Id, result.Value!.Id);
        Assert.Equal("Test Corp", result.Value.CompanyName);
    }

    [Fact]
    public async Task HandleAsync_RecordBelongsToOtherUser_ReturnsNotFound()
    {
        // Arrange
        var entity = await SeedAsync();
        var query = new GetHiringProcessByIdQuery(entity.Id, Guid.NewGuid());

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NotFound", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_NonExistentId_ReturnsNotFound()
    {
        // Act
        var result = await _handler.HandleAsync(new GetHiringProcessByIdQuery(Guid.NewGuid(), _userId));

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NotFound", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_ReturnedDtoHasCorrectHiringStages()
    {
        // Arrange
        var entity = await SeedAsync();
        entity.HiringStages = ["Applied", "Phone Screen"];
        await _db.SaveChangesAsync();

        var query = new GetHiringProcessByIdQuery(entity.Id, _userId);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.HiringStages.Count);
        Assert.Contains("Applied", result.Value.HiringStages);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    private async Task<HiringProcessEntity> SeedAsync()
    {
        var entity = new HiringProcessEntity
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            CompanyName = "Test Corp",
            ContactChannel = "LinkedIn",
            HiringStagesRaw = string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _db.HiringProcesses.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}
