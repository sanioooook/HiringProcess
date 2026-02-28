using HiringProcess.Api.Features.HiringProcesses.Models;
using HiringProcess.Api.Features.HiringProcesses.Queries;
using HiringProcess.Tests.Common;

namespace HiringProcess.Tests.Features.HiringProcesses;

/// <summary>
/// Tests for GetHiringProcessesHandler — pagination, filtering, sorting, and ownership isolation.
/// </summary>
public sealed class GetHiringProcessesHandlerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly SqliteConnection _connection;
    private readonly GetHiringProcessesHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _otherUserId = Guid.NewGuid();

    public GetHiringProcessesHandlerTests()
    {
        (_db, _connection) = TestDbContextFactory.Create();
        _handler = new GetHiringProcessesHandler(_db);
    }

    [Fact]
    public async Task HandleAsync_ReturnsOnlyCurrentUserRecords()
    {
        // Arrange — seed records for two users
        await SeedAsync(_userId, "Acme");
        await SeedAsync(_otherUserId, "Other Corp");

        var query = new GetHiringProcessesQuery(_userId);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal("Acme", result.Value.Items[0].CompanyName);
    }

    [Fact]
    public async Task HandleAsync_PaginationWorks()
    {
        // Arrange — seed 5 records
        for (var i = 1; i <= 5; i++)
            await SeedAsync(_userId, $"Company {i}");

        var query = new GetHiringProcessesQuery(_userId, Page: 2, PageSize: 2);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Items.Count);
        Assert.Equal(5, result.Value.TotalCount);
        Assert.Equal(3, result.Value.TotalPages);
    }

    [Fact]
    public async Task HandleAsync_SearchFiltersByCompanyName()
    {
        // Arrange
        await SeedAsync(_userId, "Google Inc");
        await SeedAsync(_userId, "Meta Platforms");

        var query = new GetHiringProcessesQuery(_userId, SearchTerm: "google");

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal("Google Inc", result.Value.Items[0].CompanyName);
    }

    [Fact]
    public async Task HandleAsync_FilterByCurrentStage()
    {
        // Arrange
        await SeedAsync(_userId, "Corp A", currentStage: "Interview");
        await SeedAsync(_userId, "Corp B", currentStage: "Applied");

        var query = new GetHiringProcessesQuery(_userId, CurrentStage: "Interview");

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal("Corp A", result.Value.Items[0].CompanyName);
    }

    [Fact]
    public async Task HandleAsync_InvalidPageSize_ReturnsValidationError()
    {
        var query = new GetHiringProcessesQuery(_userId, PageSize: 0);
        var result = await _handler.HandleAsync(query);

        Assert.True(result.IsFailure);
        Assert.Equal("Validation", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_EmptyDb_ReturnsEmptyPagedResult()
    {
        var query = new GetHiringProcessesQuery(_userId);
        var result = await _handler.HandleAsync(query);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.Items);
        Assert.Equal(0, result.Value.TotalCount);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    private async Task SeedAsync(Guid userId, string companyName, string? currentStage = null)
    {
        _db.HiringProcesses.Add(new HiringProcessEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyName = companyName,
            ContactChannel = "Email",
            CurrentStage = currentStage,
            HiringStagesRaw = string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }
}
